using HarmonyLib;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;
using TaleWorlds.InputSystem;

namespace SummarizeCashflow
{
	public struct LineGroup
	{
		public string Label;
		public List<string> Matches;
		public bool CaseSensitive;
		public float Gold;

		public LineGroup(string label, List<string> matches = null, bool caseSensitive = false, float gold = 0f)
		{
			Label = label;
			Matches = matches;
			CaseSensitive = caseSensitive;
			Gold = gold;
		}
	}

	[HarmonyPatch(typeof(DefaultClanFinanceModel), "CalculateClanGoldChange")]
	internal class PatchDefaultClanFinanceModel
	{
		public static bool Prefix(DefaultClanFinanceModel __instance, ref float __result, Clan clan, StatExplainer explanation = null, bool applyWithdrawals = false)
		{
			bool alt = Input.IsKeyDown(InputKey.LeftAlt) || Input.IsKeyDown(InputKey.RightAlt);

			if (clan == Clan.PlayerClan && !alt)
			{
				ExplainedNumber goldChange = new ExplainedNumber(0.0f, explanation);

				// Income
				StatExplainer incomeExplaination = new StatExplainer();
				ExplainedNumber incomeNumber = new ExplainedNumber(0.0f, incomeExplaination);
				__instance.CalculateClanIncome(clan, ref incomeNumber, applyWithdrawals);

				List<LineGroup> incomeGroups = new List<LineGroup>();
				if (Settings.IncomeGroups.Count > 0)
					incomeGroups.AddRange(Settings.IncomeGroups);

				LineGroup[] incomeGroupArray = incomeGroups.ToArray();
				float otherIncome = 0f;

				foreach (StatExplainer.ExplanationLine line in incomeExplaination.Lines)
				{
					string desc = line.Name.ToLower();
					bool matched = false;

					for (int i = 0; i < incomeGroupArray.Length; i++)
					{
						string matchAgainst = (incomeGroupArray[i].CaseSensitive ? line.Name : desc);
						foreach (string match in incomeGroups[i].Matches)
						{
							if (matchAgainst.Contains(match))
							{
								incomeGroupArray[i].Gold += line.Number;
								matched = true;
								break;
							}
						}
						if (matched)
							break;
					}
					if (!matched)
					{
						otherIncome += line.Number;
						if (Settings.DebugModeEnabled)
							InformationManager.DisplayMessage(new InformationMessage("Unknown Income: " + line.Name));
					}
				}

				foreach (LineGroup incomeGroup in incomeGroupArray)
					if (incomeGroup.Gold != 0f)
						goldChange.Add(incomeGroup.Gold, new TextObject(incomeGroup.Label));

				if (otherIncome != 0f)
					goldChange.Add(otherIncome, new TextObject("Miscellaneous Income"));

				// Kingdom Tribute. Directly lifted from DefaultClanFinanceModel.CalculateKingdomTribute()
				if (clan.Kingdom != null)
				{
					ExplainedNumber tributeNumber = new ExplainedNumber(0.0f);
					foreach (KingdomTribute kingdomTribute in clan.Kingdom.KingdomTributes)
						tributeNumber.AddFactor((float)(-kingdomTribute.Percentage * 0.01), new TextObject("{=AtFv5RMW}Kingdom Tribute"));
					goldChange.Add(tributeNumber.ResultNumber, new TextObject("Kingdom Tributes"));
				}

				// Expenses
				StatExplainer expenseExplanation = new StatExplainer();
				ExplainedNumber expenseNumber = new ExplainedNumber(0.0f, expenseExplanation);
				__instance.CalculateClanExpenses(clan, ref expenseNumber, applyWithdrawals);

				string playerName   = Hero.MainHero.Name.ToString();
				string playerNamePosessive = playerName + (playerName.EndsWith("s") ? "'" : "'s");

				List<LineGroup> expenseGroups = new List<LineGroup>()
				{
					new LineGroup(playerNamePosessive + " Party Wages", new List<string> { playerName }, true)
				};

				if (Settings.ExpenseGroups.Count > 0)
					expenseGroups.AddRange(Settings.ExpenseGroups);

				LineGroup[] expenseGroupArray = expenseGroups.ToArray();
				float otherExpenses = 0f;

				foreach (StatExplainer.ExplanationLine line in expenseExplanation.Lines)
				{
					string desc = line.Name.ToLower();
					bool matched = false;

					for (int i = 0; i < expenseGroupArray.Length; i++)
					{
						string matchAgainst = (expenseGroupArray[i].CaseSensitive ? line.Name : desc);
						foreach (string match in expenseGroupArray[i].Matches)
						{
							if (matchAgainst.Contains(match))
							{
								expenseGroupArray[i].Gold += line.Number;
								matched = true;
								break;
							}
						}
						if (matched)
							break;
					}
					if (!matched)
					{
						otherExpenses += line.Number;
						if (Settings.DebugModeEnabled)
							InformationManager.DisplayMessage(new InformationMessage("Unknown Expense: " + line.Name));
					}
				}

				foreach (LineGroup expenseGroup in expenseGroupArray)
					if (expenseGroup.Gold != 0f)
						goldChange.Add(expenseGroup.Gold, new TextObject(expenseGroup.Label));

				if (otherExpenses != 0f)
					goldChange.Add(otherExpenses, new TextObject("Miscellaneous Expenses"));

				__result = goldChange.ResultNumber;

				return false;
			}

			return true;
		}
	}
}