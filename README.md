# [Summarize Cashflow](https://www.nexusmods.com/mountandblade2bannerlord/mods/505)
A mod for Mount &amp; Blade II: Bannerlord to summarize the clan finance tooltip.
_____________________
Later in the game, and especially if you have other mods installed that increase workshop and companion limits, your cashflow tooltip might start to get a bit cluttered. Or a lot cluttered. When mine started to overflow the screen, I decided to make this mod to summarize it.
_____________________
You can also hold Alt before hovering over your denar total to see the full original accounting of your books instead of the summary.

If you see income or expenses getting lumped into a Miscellaneous category, it could be the mod isn't recognizing that type of item. Enabling Debug Mode in the Settings.xml file in this module's directory will output the descriptions for the unrecognized items into the message log. If these are coming from the base game rather than another mod, please report them and I'll make sure that they get handled and listed appropriately.

Compatible by default with the Buy Patrols and Entrepreneur mods, and any other mods can be easily supported by adding a few lines to the config file!


## Game Versions:
- ☑ e1.4.0 BETA
- ☑ e1.3.0


## Installation:
- Preferably use Vortex, but if you can't, copy the SummarizeCashflow folder extracted from the zip archive into Bannerlord's "Modules" directory.
- You may need to unblock the two .dll files (0Harmony.dll and SummarizeCashflow.dll) found in this mod's "bin/Win64_Shipping_Client" directory.


## Configuration:
- In the Settings.xml file, in the \<Entries\> node, you'll find separate configuration for \<Income\> and \<Expenses\>.
- \<Group\>s inside these nodes can be added, removed and rearranged as you want. Add a new one to add support for a mod.
- There are examples of mod compatibility support, those being Buy Patrols and Entrepreneur.
- Each \<Group\> should have a \<Label\> and at least one \<Match\>.
- The \<Label\> is the description text that will accompany that grouped gold total in the tooltip.
- A \<Match\> should be lowercase and will add lines from the original tooltip into the group's total if the provided text is found in the line.


## Known Issues:
- Holding or releasing Alt while already hovering over the tooltip doesn't change it to the other version. This is probably not going to be fixed as it would require ripping out and replacing too much of the nuts and bolts in the tooltip systems, and would run the risk of increased compatibility issues and much higher maintenance to keep it up to date with new game patches. As it is right now, it is very lightweight and unobtrusive, and should largely be compatible with any other mods that add income or expenditure items to this tooltip, though they will likely appear as Miscellaneous until the mod is updated.


## Possible Future Additions:
- Configuration options for alternate views such as listing individual workshop/caravan/party NET profits.
