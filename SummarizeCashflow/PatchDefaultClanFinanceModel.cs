using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;
using TaleWorlds.InputSystem;

namespace SummarizeCashflow
{

	[HarmonyPatch(typeof(DefaultClanFinanceModel), "CalculateClanGoldChange")]
	internal class PatchDefaultClanFinanceModel
	{
		public static bool Prefix(DefaultClanFinanceModel __instance, ref float __result, Clan clan, StatExplainer explanation = null, bool applyWithdrawals = false)
		{
			bool alt = Input.IsKeyDown(InputKey.LeftAlt) || Input.IsKeyDown(InputKey.RightAlt);

			if (clan == Clan.PlayerClan && !alt)
			{
				ExplainedNumber goldChange = new ExplainedNumber(0.0f, explanation);

				StatExplainer incomeExplaination = new StatExplainer();
				ExplainedNumber incomeNumber = new ExplainedNumber(0.0f, incomeExplaination);
				__instance.CalculateClanIncome(clan, ref incomeNumber, applyWithdrawals);

				float workshop		= 0f;
				float caravan		= 0f;
				float party			= 0f;
				float tax			= 0f;
				float contract		= 0f;
				float acres			= 0f; // Support for Entrepreneur mod (https://www.nexusmods.com/mountandblade2bannerlord/mods/138)
				float otherIncome	= 0f;

				foreach (StatExplainer.ExplanationLine line in incomeExplaination.Lines)
				{
					string desc = line.Name.ToLower();

					if (desc.Contains(" at "))
						workshop += line.Number;
					else if (desc.Contains("caravan"))
						caravan += line.Number;
					else if (desc.Contains("party"))
						party += line.Number;
					else if (desc.Contains("tax") || desc.Contains("tariff"))
						tax += line.Number;
					else if (desc.Contains("mercenary"))
						contract += line.Number;
					else if (desc.Contains("acres"))
						acres += line.Number;
					else
					{
						otherIncome += line.Number;
						if (Settings.DebugModeEnabled)
							InformationManager.DisplayMessage(new InformationMessage("Unknown Income: " + line.Name));
					}
				}

				if (workshop != 0f)
					goldChange.Add(workshop, new TextObject("Workshop Profit"));
				if (caravan != 0f)
					goldChange.Add(caravan, new TextObject("Caravan Profit"));
				if (party != 0f)
					goldChange.Add(party, new TextObject("Clan Party Profit"));
				if (tax != 0f)
					goldChange.Add(tax, new TextObject("Taxes and Tariffs Collected"));
				if (contract != 0f)
					goldChange.Add(contract, new TextObject("Mercenary Earnings"));
				if (acres != 0f)
					goldChange.Add(acres, new TextObject("Revenue from Land Investments"));
				if (otherIncome != 0f)
					goldChange.Add(otherIncome, new TextObject("Miscellaneous Income"));

				if (clan.Kingdom != null)
				{
					ExplainedNumber tributeNumber = new ExplainedNumber(0.0f);
					foreach (KingdomTribute kingdomTribute in clan.Kingdom.KingdomTributes)
					{
						tributeNumber.AddFactor((float)(-kingdomTribute.Percentage * 0.01), new TextObject("{=AtFv5RMW}Kingdom Tribute"));
					}
					goldChange.Add(tributeNumber.ResultNumber, new TextObject("Tribute"));
				}

				StatExplainer expenseExplanation = new StatExplainer();
				ExplainedNumber expenseNumber = new ExplainedNumber(0.0f, expenseExplanation);
				__instance.CalculateClanExpenses(clan, ref expenseNumber, applyWithdrawals);

				string playerName	= Hero.MainHero.Name.ToString();
				float playerWages	= 0f;
				float caravanWages	= 0f;
				float partyWages	= 0f;
				float garrisonWages	= 0f;
				float finance		= 0f;
				float support		= 0f;
				float mercs			= 0f;
				float tithe			= 0f;
				float patrolWages	= 0f; // Support for Buy Patrols mod (https://www.nexusmods.com/mountandblade2bannerlord/mods/343)
				float otherExpense	= 0f;

				foreach (StatExplainer.ExplanationLine line in expenseExplanation.Lines)
				{
					string desc = line.Name.ToLower();

					if (line.Name.Contains(playerName))
						playerWages += line.Number;
					else if (desc.Contains("caravan"))
						caravanWages += line.Number;
					else if (desc.Contains("party"))
						partyWages += line.Number;
					else if (desc.Contains("garrison"))
						garrisonWages += line.Number;
					else if (desc.Contains("finance"))
						finance += line.Number;
					else if (desc.Contains("mercenary"))
						mercs += line.Number;
					else if (desc.Contains("to king"))
						tithe += line.Number;
					else if (desc.Contains("king's"))
						support += line.Number;
					else if (desc.Contains("patrol"))
						patrolWages += line.Number;
					else
					{
						otherExpense += line.Number;
						if (Settings.DebugModeEnabled)
							InformationManager.DisplayMessage(new InformationMessage("Unknown Expense: " + line.Name));
					}
				}

				if (playerWages != 0f)
				{
					string playerNamePosessive = playerName.EndsWith("s") ? "'" : "'s";
					goldChange.Add(playerWages, new TextObject(playerName + playerNamePosessive + " Party Wages"));
				}
				if (caravanWages != 0f)
					goldChange.Add(caravanWages, new TextObject("Caravan Wages"));
				if (partyWages != 0f)
					goldChange.Add(partyWages, new TextObject("Clan Party Wages"));
				if (finance != 0f)
					goldChange.Add(finance, new TextObject("Clan Party Financial Support"));
				if (garrisonWages != 0f)
					goldChange.Add(garrisonWages, new TextObject("Garrison Wages"));
				if (patrolWages != 0f)
					goldChange.Add(patrolWages, new TextObject("Patrol Wages"));
				if (support != 0f)
					goldChange.Add(support, new TextObject("Support to Kingdom Clans"));
				if (mercs != 0f)
					goldChange.Add(mercs, new TextObject("Mercenary Contracts"));
				if (tithe != 0f)
					goldChange.Add(tithe, new TextObject("Tithe to Kingdom"));
				if (otherExpense != 0f)
					goldChange.Add(otherExpense, new TextObject("Miscellaneous Expenses"));

				__result = goldChange.ResultNumber;

				return false;
			}

			return true;
		}
	}
}