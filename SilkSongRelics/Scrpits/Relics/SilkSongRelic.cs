using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using System.Reflection;
using System.Threading.Tasks;

namespace SilkSong.Scrpits.Relics
{
public abstract class SilkSongReic : CustomRelicModel
{
	protected static bool IsMultiplayerActive()
	{
		return TryGetMultiplayerRole(out _);
	}

	protected static bool IsMultiplayerActive(PlayerChoiceContext context)
	{
		if (TryIsMultiplayerFromChoiceContext(context, out bool isMultiplayer))
		{
			return isMultiplayer;
		}
		return IsMultiplayerActive();
	}

	protected bool IsMultiplayerActiveForOwner(PlayerChoiceContext context)
	{
		if (IsMultiplayerActive(context))
		{
			return true;
		}
		object? runState = Owner?.Creature?.Player?.RunState;
		return TryGetPlayerCountFromRunState(runState, out int playerCount) && playerCount > 1;
	}

	protected static bool IsMultiplayerHost()
	{
		return TryGetMultiplayerRole(out bool isHost) && isHost;
	}

	protected static bool IsMultiplayerHost(PlayerChoiceContext context)
	{
		if (TryIsHostFromChoiceContext(context, out bool isHost))
		{
			return isHost;
		}
		return IsMultiplayerHost();
	}

	private static bool TryGetMultiplayerRole(out bool isHost)
	{
		isHost = false;
		SceneTree? tree = Engine.GetMainLoop() as SceneTree;
		Node? root = tree?.Root;
		if (root == null)
		{
			return false;
		}

		if (HasNodeOfTypeNameContains(root, "NetHostGameService"))
		{
			isHost = true;
			return true;
		}
		if (HasNodeOfTypeNameContains(root, "NetClientGameService"))
		{
			isHost = false;
			return true;
		}
		return false;
	}

	private static bool HasNodeOfTypeNameContains(Node node, string typeNameContains)
	{
		string? fullName = node.GetType().FullName;
		if (fullName != null && fullName.Contains(typeNameContains, StringComparison.Ordinal))
		{
			return true;
		}
		foreach (Node child in node.GetChildren())
		{
			if (HasNodeOfTypeNameContains(child, typeNameContains))
			{
				return true;
			}
		}
		return false;
	}

	protected static bool TryQueueNetAction(PlayerChoiceContext context, INetAction action, out Task? task)
	{
		task = null;
		return TryQueueNetActionViaReflection(context, action, out task);
	}

	private static MethodInfo? _queueNetActionMethod;
	private static bool _loggedQueueMethod;
	private static bool _loggedQueueMissing;

	private static bool TryQueueNetActionViaReflection(PlayerChoiceContext context, INetAction action, out Task? task)
	{
		task = null;
		if (_queueNetActionMethod == null)
		{
			_queueNetActionMethod = FindQueueNetActionMethod(context.GetType());
			if (_queueNetActionMethod == null)
			{
				if (!_loggedQueueMissing)
				{
					_loggedQueueMissing = true;
					Log.Warn("SilkSongRelics: could not locate a NetAction queue method; right-click actions will not sync.");
					GD.PrintErr("SilkSongRelics: could not locate a NetAction queue method; right-click actions will not sync.");
				}
				return false;
			}
			if (!_loggedQueueMethod)
			{
				_loggedQueueMethod = true;
				Log.Info($"SilkSongRelics: selected NetAction queue method: {_queueNetActionMethod.DeclaringType?.FullName}.{_queueNetActionMethod.Name}");
				GD.Print($"SilkSongRelics: selected NetAction queue method: {_queueNetActionMethod.DeclaringType?.FullName}.{_queueNetActionMethod.Name}");
			}
		}

		object? result;
		try
		{
			object? target = _queueNetActionMethod.IsStatic ? null : context;
			ParameterInfo[] p = _queueNetActionMethod.GetParameters();
			Type ctxType = context.GetType();
			Type actType = action.GetType();
			if (p.Length == 2 && p[0].ParameterType.IsAssignableFrom(ctxType) && p[1].ParameterType.IsAssignableFrom(actType))
			{
				result = _queueNetActionMethod.Invoke(target, [context, action]);
			}
			else if (p.Length == 2 && p[0].ParameterType.IsAssignableFrom(actType) && p[1].ParameterType.IsAssignableFrom(ctxType))
			{
				result = _queueNetActionMethod.Invoke(target, [action, context]);
			}
			else if (p.Length == 1 && p[0].ParameterType.IsAssignableFrom(actType))
			{
				result = _queueNetActionMethod.Invoke(target, [action]);
			}
			else
			{
				return false;
			}
		}
		catch (Exception e)
		{
			Log.Warn($"SilkSongRelics: NetAction queue invoke failed: {_queueNetActionMethod.DeclaringType?.FullName}.{_queueNetActionMethod.Name} :: {e.GetType().Name} {e.Message}");
			GD.PrintErr($"SilkSongRelics: NetAction queue invoke failed: {_queueNetActionMethod.DeclaringType?.FullName}.{_queueNetActionMethod.Name} :: {e.GetType().Name} {e.Message}");
			return false;
		}

		task = result as Task;
		return true;
	}

	private static MethodInfo? FindQueueNetActionMethod(Type contextRuntimeType)
	{
		Type ctx = typeof(PlayerChoiceContext);
		Type act = typeof(INetAction);

		MethodInfo? best = null;
		int bestScore = -1;

		foreach (MethodInfo m in contextRuntimeType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
		{
			int score = ScoreCandidate(m, ctx, act);
			if (score > bestScore)
			{
				bestScore = score;
				best = m;
			}
		}
		if (best != null && bestScore >= 100)
		{
			return best;
		}

		if (!_loggedQueueMissing)
		{
			List<string> candidates = new List<string>();
			foreach (MethodInfo m in contextRuntimeType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				if (m.Name.Contains("NetAction", StringComparison.OrdinalIgnoreCase) || m.Name.Contains("Queue", StringComparison.OrdinalIgnoreCase) || m.Name.Contains("Enqueue", StringComparison.OrdinalIgnoreCase) || m.Name.Contains("Send", StringComparison.OrdinalIgnoreCase))
				{
					string sig = $"{m.DeclaringType?.FullName}.{m.Name}({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))})";
					candidates.Add(sig);
				}
			}
			candidates.Sort(StringComparer.Ordinal);
			for (int i = 0; i < Math.Min(15, candidates.Count); i++)
			{
				Log.Info($"SilkSongRelics: PlayerChoiceContext candidate: {candidates[i]}");
			}
		}

		foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
		{
			string? asmName = asm.GetName().Name;
			if (asmName == null || !asmName.Contains("sts2", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			Type[] types;
			try
			{
				types = asm.GetTypes();
			}
			catch
			{
				continue;
			}
			foreach (Type t in types)
			{
				foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
				{
					int score = ScoreCandidate(m, ctx, act);
					if (score > bestScore)
					{
						bestScore = score;
						best = m;
					}
				}
			}
		}
		return best;
	}

	private static int ScoreCandidate(MethodInfo m, Type ctx, Type act)
	{
		if (!m.Name.Contains("NetAction", StringComparison.OrdinalIgnoreCase) &&
			!m.Name.Contains("Queue", StringComparison.OrdinalIgnoreCase) &&
			!m.Name.Contains("Enqueue", StringComparison.OrdinalIgnoreCase) &&
			!m.Name.Contains("Send", StringComparison.OrdinalIgnoreCase))
		{
			return -1;
		}

		ParameterInfo[] p = m.GetParameters();
		bool isNetActionParam(Type pt)
		{
			return pt == typeof(object) || act.IsAssignableFrom(pt);
		}
		bool signatureOk =
			(p.Length == 2 && p[0].ParameterType.IsAssignableFrom(ctx) && isNetActionParam(p[1].ParameterType)) ||
			(p.Length == 2 && isNetActionParam(p[0].ParameterType) && p[1].ParameterType.IsAssignableFrom(ctx)) ||
			(p.Length == 1 && isNetActionParam(p[0].ParameterType));

		if (!signatureOk)
		{
			return -1;
		}

		int score = 0;
		string? ns = m.DeclaringType?.Namespace ?? string.Empty;
		string dn = m.DeclaringType?.Name ?? string.Empty;
		if (ns.Contains("Multiplayer", StringComparison.OrdinalIgnoreCase))
		{
			score += 50;
		}
		if (dn.Contains("Multiplayer", StringComparison.OrdinalIgnoreCase) || dn.Contains("Net", StringComparison.OrdinalIgnoreCase))
		{
			score += 30;
		}
		if (m.Name.Contains("QueueNetAction", StringComparison.OrdinalIgnoreCase) || m.Name.Contains("EnqueueNetAction", StringComparison.OrdinalIgnoreCase))
		{
			score += 100;
		}
		if (m.Name.Equals("QueueNetAction", StringComparison.OrdinalIgnoreCase) || m.Name.Equals("EnqueueNetAction", StringComparison.OrdinalIgnoreCase))
		{
			score += 80;
		}
		if (m.Name.Contains("Send", StringComparison.OrdinalIgnoreCase) && m.Name.Contains("Host", StringComparison.OrdinalIgnoreCase))
		{
			score += 80;
		}
		if (p.Length == 2)
		{
			score += 20;
		}
		if (m.ReturnType == typeof(Task) || typeof(Task).IsAssignableFrom(m.ReturnType))
		{
			score += 10;
		}
		return score;
	}

	private static bool TryIsMultiplayerFromChoiceContext(PlayerChoiceContext context, out bool isMultiplayer)
	{
		isMultiplayer = false;
		Type t = context.GetType();
		PropertyInfo? p =
			t.GetProperty("IsMultiplayer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ??
			t.GetProperty("InMultiplayer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		if (p != null && p.PropertyType == typeof(bool))
		{
			isMultiplayer = (bool)(p.GetValue(context) ?? false);
			return true;
		}

		PropertyInfo? g =
			t.GetProperty("MultiplayerGame", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ??
			t.GetProperty("NetGame", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ??
			t.GetProperty("Multiplayer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		if (g != null)
		{
			isMultiplayer = g.GetValue(context) != null;
			return true;
		}
		return false;
	}

	private static bool TryIsHostFromChoiceContext(PlayerChoiceContext context, out bool isHost)
	{
		isHost = false;
		Type t = context.GetType();
		PropertyInfo? p =
			t.GetProperty("IsHost", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ??
			t.GetProperty("IsMultiplayerHost", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (p != null && p.PropertyType == typeof(bool))
		{
			isHost = (bool)(p.GetValue(context) ?? false);
			return true;
		}
		return false;
	}

	private static bool TryGetPlayerCountFromRunState(object? runState, out int count)
	{
		count = 0;
		if (runState == null)
		{
			return false;
		}

		Type t = runState.GetType();
		PropertyInfo? playersProp = t.GetProperty("Players", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		object? players = playersProp?.GetValue(runState);
		if (players is System.Collections.ICollection c)
		{
			count = c.Count;
			return true;
		}
		if (players is System.Collections.IEnumerable e)
		{
			int n = 0;
			foreach (object _ in e)
			{
				n++;
				if (n > 8)
				{
					break;
				}
			}
			count = n;
			return true;
		}
		return false;
	}

	protected enum RightClickNetKind : byte
	{
		ReserveBind = 1,
		Cogfly = 2,
		ToolRelic = 3
	}

	protected sealed class RightClickNetAction : INetAction
	{
		public string RelicIdEntry { get; private set; } = string.Empty;
		public RightClickNetKind Kind { get; private set; }

		public RightClickNetAction() { }

		public RightClickNetAction(RightClickNetKind kind, string relicIdEntry)
		{
			Kind = kind;
			RelicIdEntry = relicIdEntry;
		}

		public GameAction ToGameAction(Player player)
		{
			return new RightClickGameAction(player, Kind, RelicIdEntry);
		}

		public void Serialize(PacketWriter writer)
		{
			writer.WriteByte((byte)Kind);
			writer.WriteString(RelicIdEntry);
		}

		public void Deserialize(PacketReader reader)
		{
			Kind = (RightClickNetKind)reader.ReadByte();
			RelicIdEntry = reader.ReadString() ?? string.Empty;
		}
	}

	private sealed class RightClickGameAction : GameAction
	{
		private readonly Player player;
		private readonly RightClickNetKind kind;
		private readonly string relicIdEntry;

		public RightClickGameAction(Player player, RightClickNetKind kind, string relicIdEntry)
		{
			this.player = player;
			this.kind = kind;
			this.relicIdEntry = relicIdEntry;
		}

		public override INetAction ToNetAction()
		{
			return new RightClickNetAction(kind, relicIdEntry);
		}

		protected override Task ExecuteAction()
		{
			return kind switch
			{
				RightClickNetKind.ReserveBind => ExecuteReserveBind(),
				RightClickNetKind.Cogfly => ExecuteCogfly(),
				RightClickNetKind.ToolRelic => ExecuteToolRelic(),
				_ => Task.CompletedTask
			};
		}

		private Task ExecuteReserveBind()
		{
			global::SilkSongRelics.Scrpits.Relics.ReserveBind? relic = FindRelic<global::SilkSongRelics.Scrpits.Relics.ReserveBind>();
			if (relic == null || relic.IsUsedUp)
			{
				return Task.CompletedTask;
			}
			relic.UsedUp = true;
			relic.Flash();
			return MegaCrit.Sts2.Core.Commands.PlayerCmd.SetEnergy(player.MaxEnergy, player);
		}

		private Task ExecuteCogfly()
		{
			global::SilkSongRelics.Scrpits.Relics.Cogfly? relic = FindRelic<global::SilkSongRelics.Scrpits.Relics.Cogfly>();
			if (relic == null || relic.IsUsedUp)
			{
				return Task.CompletedTask;
			}
			relic.Flash();
			relic.ToolCount--;
			return MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<global::SilkSongRelics.Scrpits.Powers.CogflyPower>(player.Creature, 1, player.Creature, null);
		}

		private async Task ExecuteToolRelic()
		{
			global::SilkSong.Scrpits.Relics.ToolRelic? relic = FindRelic<global::SilkSong.Scrpits.Relics.ToolRelic>();
			if (relic == null || relic.IsUsedUp)
			{
				return;
			}

			relic.Flash();
			relic.ToolCount--;
			List<MegaCrit.Sts2.Core.Models.CardModel> list = new List<MegaCrit.Sts2.Core.Models.CardModel>();
			list.Add(relic.ToolCard);
			await MegaCrit.Sts2.Core.Commands.CardPileCmd.AddGeneratedCardsToCombat(list, MegaCrit.Sts2.Core.Entities.Cards.PileType.Hand, addedByPlayer: true);
		}

		private T? FindRelic<T>() where T : MegaCrit.Sts2.Core.Models.RelicModel
		{
			foreach (MegaCrit.Sts2.Core.Models.RelicModel relic in player.Relics)
			{
				if (relic is T match && match.Id.Entry == relicIdEntry)
				{
					return match;
				}
			}
			return null;
		}

		public override ulong OwnerId => player.NetId;

		public override MegaCrit.Sts2.Core.Entities.Multiplayer.GameActionType ActionType => MegaCrit.Sts2.Core.Entities.Multiplayer.GameActionType.Combat;
	}
    
    
   // protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
    public override string PackedIconPath => $"res://SilkSongRelics/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://SilkSongRelics/ArtWorks/Relics/{Id.Entry}.png";
    protected override string BigIconPath => $"res://SilkSongRelics/ArtWorks/Relics/{Id.Entry}.png";
   
}
}
