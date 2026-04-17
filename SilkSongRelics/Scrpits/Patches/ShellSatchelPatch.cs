// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Reflection.Emit;
// using BaseLib.Utils;
// using HarmonyLib;
// using MegaCrit.Sts2.Core.Audio.Debug;
// using MegaCrit.Sts2.Core.Combat;
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.ControllerInput;
// using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.Entities.Players;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.Helpers;
// using MegaCrit.Sts2.Core.Hooks;
// using MegaCrit.Sts2.Core.Localization;
// using MegaCrit.Sts2.Core.Models;
// using MegaCrit.Sts2.Core.Nodes.Cards;
// using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
// using MegaCrit.Sts2.Core.Nodes.Combat;
// using MegaCrit.Sts2.Core.Nodes.CommonUi;
// using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
// using MegaCrit.Sts2.Core.Nodes.Rooms;
// using MegaCrit.Sts2.Core.Random;
// using MegaCrit.Sts2.Core.Runs;
// using SilkSongRelics.Scrpits.Relics;

// namespace SilkSong.Scrpits.Patches;

// [HarmonyPatch]
// public static class ShellSatchelPatch
// {
// 	private static MethodBase TargetMethod()
// 	{
// 		MethodInfo? match = AccessTools.GetDeclaredMethods(typeof(CardPileCmd))
// 			.Where(m => m.Name == nameof(CardPileCmd.Draw))
// 			.Where(m => m.GetParameters().Length == 4)
// 			.Where(m =>
// 			{
// 				ParameterInfo[] p = m.GetParameters();
// 				return typeof(PlayerChoiceContext).IsAssignableFrom(p[0].ParameterType)
// 					&& p[2].ParameterType == typeof(Player)
// 					&& p[3].ParameterType == typeof(bool);
// 			})
// 			.SingleOrDefault();

// 		return match ?? throw new MissingMethodException("Could not locate CardPileCmd.Draw(PlayerChoiceContext, ?, Player, bool) overload to patch.");
// 	}

// 	internal static int GetMaxHandSize(Player player)
// 	{
// 		if (player.GetRelic<ShellSatchel>() != null)
// 		{
// 			return 12;
// 		}
// 		return 10;
// 	}

// 	private static int GetMaxHandSizeFromHandCards(IReadOnlyList<CardModel> handCards)
// 	{
// 		Player? owner = handCards.FirstOrDefault()?.Owner;
// 		if (owner != null && owner.GetRelic<ShellSatchel>() != null)
// 		{
// 			return 12;
// 		}
// 		return 10;
// 	}

// 	private static bool Prefix(PlayerChoiceContext choiceContext, decimal count, Player player, bool fromHandDraw, ref Task<IEnumerable<CardModel>> __result)
// 	{
// 		if (player.GetRelic<ShellSatchel>() == null)
// 		{
// 			return true;
// 		}

// 		__result = DrawWithHandCap(choiceContext, count, player, fromHandDraw);
// 		return false;
// 	}

// 	internal static async Task<IEnumerable<CardModel>> DrawWithHandCap(PlayerChoiceContext choiceContext, decimal count, Player player, bool fromHandDraw)
// 	{
// 		if (CombatManager.Instance.IsOverOrEnding)
// 		{
// 			return Array.Empty<CardModel>();
// 		}

// 		if (!Hook.ShouldDraw(player.Creature.CombatState, player, fromHandDraw, out AbstractModel modifier))
// 		{
// 			await Hook.AfterPreventingDraw(player.Creature.CombatState, modifier);
// 			return Array.Empty<CardModel>();
// 		}

// 		int maxHandSize = GetMaxHandSize(player);
// 		CombatState combatState = player.Creature.CombatState;
// 		List<CardModel> result = new List<CardModel>();
// 		CardPile hand = PileType.Hand.GetPile(player);
// 		CardPile drawPile = PileType.Draw.GetPile(player);
// 		int drawsRequested = (count > 0m) ? (int)Math.Ceiling(count) : 0;
// 		if (drawsRequested == 0)
// 		{
// 			return result;
// 		}

// 		int num = Math.Max(0, maxHandSize - hand.Cards.Count);
// 		if (num == 0)
// 		{
// 			CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player, maxHandSize);
// 			return result;
// 		}

// 		for (int i = 0; i < drawsRequested; i++)
// 		{
// 			if (num <= 0)
// 			{
// 				break;
// 			}

// 			if (!CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player, maxHandSize))
// 			{
// 				break;
// 			}

// 			await CardPileCmd.ShuffleIfNecessary(choiceContext, player);
// 			if (!CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player, maxHandSize))
// 			{
// 				break;
// 			}

// 			CardModel? card = drawPile.Cards.FirstOrDefault();
// 			if (card == null || hand.Cards.Count >= maxHandSize)
// 			{
// 				break;
// 			}

// 			result.Add(card);
// 			await CardPileCmd.Add(card, hand);
// 			CombatManager.Instance.History.CardDrawn(combatState, card, fromHandDraw);
// 			await Hook.AfterCardDrawn(combatState, choiceContext, card, fromHandDraw);
// 			card.InvokeDrawn();
// 			NDebugAudioManager.Instance?.Play("card_deal.mp3", 0.25f, PitchVariance.Small);
// 			num = Math.Max(0, maxHandSize - hand.Cards.Count);
// 		}

// 		return result;
// 	}

// 	private static bool CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(Player player, int maxHandSize)
// 	{
// 		if (PileType.Draw.GetPile(player).Cards.Count + PileType.Discard.GetPile(player).Cards.Count == 0)
// 		{
// 			ThinkCmd.Play(new LocString("combat_messages", "NO_DRAW"), player.Creature, 2.0);
// 			return false;
// 		}
// 		if (PileType.Hand.GetPile(player).Cards.Count >= maxHandSize)
// 		{
// 			ThinkCmd.Play(new LocString("combat_messages", "HAND_FULL"), player.Creature, 2.0);
// 			return false;
// 		}
// 		return true;
// 	}
// }

// [HarmonyPatch]
// public static class ShellSatchelPatch_SingleDraw
// {
// 	private static MethodBase TargetMethod()
// 	{
// 		MethodInfo? match = AccessTools.GetDeclaredMethods(typeof(CardPileCmd))
// 			.Where(m => m.Name == nameof(CardPileCmd.Draw))
// 			.Where(m => m.GetParameters().Length == 2)
// 			.Where(m =>
// 			{
// 				ParameterInfo[] p = m.GetParameters();
// 				return typeof(PlayerChoiceContext).IsAssignableFrom(p[0].ParameterType)
// 					&& p[1].ParameterType == typeof(Player);
// 			})
// 			.SingleOrDefault();

// 		return match ?? throw new MissingMethodException("Could not locate CardPileCmd.Draw(PlayerChoiceContext, Player) overload to patch.");
// 	}

// 	private static bool Prefix(PlayerChoiceContext choiceContext, Player player, ref Task<CardModel?> __result)
// 	{
// 		if (player.GetRelic<ShellSatchel>() == null)
// 		{
// 			return true;
// 		}

// 		__result = DrawOneWithHandCap(choiceContext, player);
// 		return false;
// 	}

// 	private static async Task<CardModel?> DrawOneWithHandCap(PlayerChoiceContext choiceContext, Player player)
// 	{
// 		return (await ShellSatchelPatch.DrawWithHandCap(choiceContext, 1m, player, fromHandDraw: false)).FirstOrDefault();
// 	}
// }

// [HarmonyPatch]
// public static class ShellSatchelPatch_AddToHandCapacity
// {
// 	private static MethodBase TargetMethod()
// 	{
// 		MethodInfo? match = AccessTools.GetDeclaredMethods(typeof(CardPileCmd))
// 			.Where(m => m.Name == nameof(CardPileCmd.Add))
// 			.Where(m => m.GetParameters().Length == 5)
// 			.Where(m =>
// 			{
// 				ParameterInfo[] p = m.GetParameters();
// 				return p[0].ParameterType == typeof(IEnumerable<CardModel>)
// 					&& p[1].ParameterType == typeof(CardPile)
// 					&& p[2].ParameterType == typeof(CardPilePosition)
// 					&& typeof(AbstractModel).IsAssignableFrom(p[3].ParameterType)
// 					&& p[4].ParameterType == typeof(bool);
// 			})
// 			.SingleOrDefault();

// 		return match ?? throw new MissingMethodException("Could not locate CardPileCmd.Add(IEnumerable<CardModel>, CardPile, CardPilePosition, AbstractModel, bool) overload to patch.");
// 	}

// 	private static void Postfix(IEnumerable<CardModel> cards, CardPile newPile, CardPilePosition position, AbstractModel? source, bool skipVisuals, ref Task<IReadOnlyList<CardPileAddResult>> __result)
// 	{
// 		if (newPile.Type != PileType.Hand)
// 		{
// 			return;
// 		}

// 		Task<IReadOnlyList<CardPileAddResult>> original = __result;
// 		__result = PostProcessHandAdds(original, cards);
// 	}

// 	private static async Task<IReadOnlyList<CardPileAddResult>> PostProcessHandAdds(Task<IReadOnlyList<CardPileAddResult>> originalTask, IEnumerable<CardModel> cards)
// 	{
// 		IReadOnlyList<CardPileAddResult> results = await originalTask;

// 		CardModel? first = cards.FirstOrDefault();
// 		Player? player = first?.Owner;
// 		if (player == null || player.GetRelic<ShellSatchel>() == null)
// 		{
// 			return results;
// 		}

// 		int maxHandSize = ShellSatchelPatch.GetMaxHandSize(player);
// 		CardPile hand = PileType.Hand.GetPile(player);
// 		CardPile discard = PileType.Discard.GetPile(player);

// 		foreach (CardModel card in cards)
// 		{
// 			if (hand.Cards.Count >= maxHandSize)
// 			{
// 				break;
// 			}

// 			if (card.Owner != player)
// 			{
// 				continue;
// 			}

// 			CardPile? currentPile = card.Pile;
// 			if (currentPile == null || currentPile.Type != PileType.Discard)
// 			{
// 				continue;
// 			}

// 			if (hand.Cards.Contains(card))
// 			{
// 				continue;
// 			}

// 			if (discard.Cards.Contains(card))
// 			{
// 				discard.RemoveInternal(card, silent: false);
// 				hand.AddInternal(card, -1, silent: false);

// 				if (CombatManager.Instance.IsInProgress && NCombatRoom.Instance?.Ui?.Hand != null)
// 				{
// 					NCard? nCard = NCard.Create(card);
// 					if (nCard != null)
// 					{
// 						NCombatRoom.Instance.Ui.AddChild(nCard);
// 						nCard.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
// 						int idx = -1;
// 						IReadOnlyList<CardModel> handCards = hand.Cards;
// 						for (int k = 0; k < handCards.Count; k++)
// 						{
// 							if (handCards[k] == card)
// 							{
// 								idx = k;
// 								break;
// 							}
// 						}
// 						NPlayerHand handUi = NCombatRoom.Instance.Ui.Hand;
// 						nCard.GlobalPosition = handUi.GlobalPosition + handUi.Size * 0.5f;
// 						NHandCardHolder holder = handUi.Add(nCard, idx);
// 						holder.Position = holder.TargetPosition;
// 						holder.Hitbox.SetEnabled(enabled: true);
// 					}
// 				}
// 			}
// 		}

// 		return results;
// 	}
// }

// [HarmonyPatch]
// public static class ShellSatchelPatch_HandFullCheck
// {
// 	private static MethodBase TargetMethod()
// 	{
// 		MethodInfo? match = AccessTools.GetDeclaredMethods(typeof(CardPileCmd))
// 			.Where(m => m.Name == "CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot")
// 			.Where(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(Player))
// 			.SingleOrDefault();

// 		return match ?? throw new MissingMethodException("Could not locate CardPileCmd.CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(Player) to patch.");
// 	}

// 	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
// 	{
// 		MethodInfo getMaxHandSize = AccessTools.Method(typeof(ShellSatchelPatch), "GetMaxHandSize");

// 		List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
// 		for (int i = 0; i < codes.Count; i++)
// 		{
// 			CodeInstruction ci = codes[i];
// 			bool isTen = ci.opcode == OpCodes.Ldc_I4_S && ci.operand is sbyte sb && sb == 10;
// 			isTen |= ci.opcode == OpCodes.Ldc_I4 && ci.operand is int iv && iv == 10;
// 			if (!isTen)
// 			{
// 				continue;
// 			}

// 			List<Label> labels = ci.labels;
// 			List<ExceptionBlock> blocks = ci.blocks;

// 			codes[i] = new CodeInstruction(OpCodes.Ldarg_0)
// 			{
// 				labels = labels,
// 				blocks = blocks
// 			};
// 			codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, getMaxHandSize));
// 			i++;
// 		}

// 		return codes;
// 	}
// }

// [HarmonyPatch(typeof(CardConsoleCmd), nameof(CardConsoleCmd.Process))]
// public static class ShellSatchelPatch_CardConsoleCmdHandCap
// {
// 	private static int GetMaxHandSizeNullable(Player? player)
// 	{
// 		if (player != null && player.GetRelic<ShellSatchel>() != null)
// 		{
// 			return 12;
// 		}
// 		return 10;
// 	}

// 	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
// 	{
// 		MethodInfo getMaxHandSize = AccessTools.Method(typeof(ShellSatchelPatch_CardConsoleCmdHandCap), nameof(GetMaxHandSizeNullable));

// 		List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
// 		for (int i = 0; i < codes.Count; i++)
// 		{
// 			CodeInstruction ci = codes[i];
// 			bool isTen = ci.opcode == OpCodes.Ldc_I4_S && ci.operand is sbyte sb && sb == 10;
// 			isTen |= ci.opcode == OpCodes.Ldc_I4 && ci.operand is int iv && iv == 10;

// 			if (!isTen)
// 			{
// 				continue;
// 			}

// 			List<Label> labels = ci.labels;
// 			List<ExceptionBlock> blocks = ci.blocks;

// 			codes[i] = new CodeInstruction(OpCodes.Ldarg_1)
// 			{
// 				labels = labels,
// 				blocks = blocks
// 			};
// 			codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, getMaxHandSize));
// 			i++;
// 		}

// 		return codes;
// 	}
// }

// [HarmonyPatch(typeof(HandPosHelper), nameof(HandPosHelper.GetPosition), new[] { typeof(int), typeof(int) })]
// public static class ShellSatchelPatch_HandPosHelper_GetPosition
// {
// 	private static bool Prefix(int handSize, int cardIndex, ref Godot.Vector2 __result)
// 	{
// 		if (handSize <= 10)
// 		{
// 			return true;
// 		}

// 		int n = Math.Max(handSize, 1);
// 		int i = Math.Clamp(cardIndex, 0, n - 1);

// 		const float spanX = 610f;
// 		const float centerY = -50f;
// 		const float arcHeight = 88f;

// 		float alpha = (n == 1) ? 0.5f : i / (float)(n - 1);
// 		float x = Godot.Mathf.Lerp(-spanX, spanX, alpha);
// 		float t = Godot.Mathf.Lerp(-1f, 1f, alpha);
// 		float y = centerY + arcHeight * t * t;

// 		__result = new Godot.Vector2(x, y);
// 		return false;
// 	}
// }

// [HarmonyPatch(typeof(HandPosHelper), nameof(HandPosHelper.GetAngle), new[] { typeof(int), typeof(int) })]
// public static class ShellSatchelPatch_HandPosHelper_GetAngle
// {
// 	private static bool Prefix(int handSize, int cardIndex, ref float __result)
// 	{
// 		if (handSize <= 10)
// 		{
// 			return true;
// 		}

// 		int n = Math.Max(handSize, 1);
// 		int i = Math.Clamp(cardIndex, 0, n - 1);

// 		float alpha = (n == 1) ? 0.5f : i / (float)(n - 1);
// 		float t = Godot.Mathf.Lerp(-1f, 1f, alpha);
// 		float maxAngle = 15f * (handSize / 10f);
// 		__result = t * maxAngle;
// 		return false;
// 	}
// }

// [HarmonyPatch(typeof(NPlayerHand), "StartCardPlay", new[] { typeof(NHandCardHolder), typeof(bool) })]
// public static class ShellSatchelPatch_NPlayerHand_StartCardPlay
// {
// 	private static readonly AccessTools.FieldRef<NPlayerHand, int> _draggedHolderIndexRef = AccessTools.FieldRefAccess<NPlayerHand, int>("_draggedHolderIndex");
// 	private static readonly AccessTools.FieldRef<NPlayerHand, Dictionary<NHandCardHolder, int>> _holdersAwaitingQueueRef = AccessTools.FieldRefAccess<NPlayerHand, Dictionary<NHandCardHolder, int>>("_holdersAwaitingQueue");
// 	private static readonly AccessTools.FieldRef<NPlayerHand, NCardPlay?> _currentCardPlayRef = AccessTools.FieldRefAccess<NPlayerHand, NCardPlay?>("_currentCardPlay");
// 	private static readonly AccessTools.FieldRef<NPlayerHand, Godot.StringName[]> _selectCardShortcutsRef = AccessTools.FieldRefAccess<NPlayerHand, Godot.StringName[]>("_selectCardShortcuts");

// 	private static readonly MethodInfo _returnHolderToHand = AccessTools.Method(typeof(NPlayerHand), "ReturnHolderToHand", new[] { typeof(NHandCardHolder) });

// 	private static bool Prefix(NPlayerHand __instance, NHandCardHolder holder, bool startedViaShortcut)
// 	{
// 		int idx = holder.GetIndex();
// 		_draggedHolderIndexRef(__instance) = idx;
// 		_holdersAwaitingQueueRef(__instance).Add(holder, idx);
// 		holder.Reparent(__instance);
// 		holder.BeginDrag();

// 		NCardPlay play = NControllerManager.Instance.IsUsingController
// 			? (NCardPlay)NControllerCardPlay.Create(holder)
// 			: (NCardPlay)NMouseCardPlay.Create(holder, GetCancelShortcut(__instance, idx), startedViaShortcut);

// 		_currentCardPlayRef(__instance) = play;
// 		__instance.AddChildSafely(play);

// 		play.Connect(NCardPlay.SignalName.Finished, Godot.Callable.From((bool success) =>
// 		{
// 			RunManager.Instance.HoveredModelTracker.OnLocalCardDeselected();
// 			if (!success)
// 			{
// 				_returnHolderToHand.Invoke(__instance, new object[] { holder });
// 			}
// 			_draggedHolderIndexRef(__instance) = -1;
// 			__instance.ForceRefreshCardIndices();
// 		}));

// 		RunManager.Instance.HoveredModelTracker.OnLocalCardSelected(holder.CardNode.Model);
// 		play.Start();
// 		__instance.ForceRefreshCardIndices();
// 		holder.SetIndexLabel(idx + 1);
// 		return false;
// 	}

// 	private static Godot.StringName GetCancelShortcut(NPlayerHand hand, int draggedHolderIndex)
// 	{
// 		Godot.StringName[] shortcuts = _selectCardShortcutsRef(hand);
// 		if (draggedHolderIndex >= 0 && draggedHolderIndex < shortcuts.Length)
// 		{
// 			return shortcuts[draggedHolderIndex];
// 		}
// 		return MegaInput.releaseCard;
// 	}
// }
