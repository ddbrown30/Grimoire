## Blood on the Clocktower - Grimoire

This program should hopefully cover everything you might need to do when playing as the Storyteller. This readme will cover the basic features of the software to get you started.

![Grimoire](/Preview.png?raw=true)

#### Select Roles:
During this step, you select the roles that your players will be playing during the game. You can filter by edition in the top left. Selecting a role will add it to the Grimoire on the right. If that role comes with any helper tokens, those will also be added. Selecting the role again will remove them. As you select roles, the role count display will update telling you how many of each role type (Townsfolk, Minions, etc.) you have selected. This is here simply so that you do not need to manually count your roles each time. Once you've selected your roles, press the Next button to continue.

#### Select Bluffs:
During the step, you will be selecting the bluff roles that will be shown to the Demon. Selected roles will be added to the far right. If you do not need any bluff roles (when playing with fewer than 7 players, for instance), you can just skip this step. Select Next when you are ready to continue.

#### First Night:
This will give you the night order for the first night and will contain all your selected roles that have first night abilities. This should be the same as the order you receive from the script tool. If you have a Spy in the game, a helpful Screenshot button will appear next to the Spy. This will take a screenshot of the entire Grimoire and open the folder it is saved to so that you can easily send it to your Spy player when playing via Discord or another similar app. When the first night is finished, select Next.

#### Other Nights:
This will give you the night order for all remaining nights. As with the First Night display, this will also have all the selected roles that have other night abilities. In addition, as players die, their roles will be automatically removed from this list.

#### Add Travelers:
Selecting the Add Travelers button will allow you to add Travelers to the game. This can be done at any time but otherwise works the same as selecting roles. When adding a Traveler, an alignment token is also automatically added.

#### The Grimoire:
Role tokens can be dragged around in order to reorganize the seating order and you can drag helper tokens around and place them however you would like. Clicking on a role token will toggle that player between alive and dead.

Right clicking in the open area of the Grimoire will open up a context menu with the following options:
* Reset Grimoire - Selecting this option will delete all tokens and reset the Grimoire back to role selection.
* Reset helper tokens - Selecting this option will reset the position of all helper tokens back to their initial positions. This is useful if you've placed a helper token somewhere you cannot reach.
* Randomize player positions - Selecting this option will randomize the position of the tokens in the seating arrangement. Since we often have no way of doing the bag draw when playing online, this randomization will allow you to assign out roles completely randomly.

Right clicking on a role token will open up a context menu with the following options:
* Change role - Selecting this option will open a dialog to select a new role for this player. Selecting a new role will delete the existing helper tokens for that player and create the tokens for the new role.
* Swap alignment - Selecting this option will create an appropriate alignment token if one doesn't exist otherwise it will swap it to the opposite alignment.

#### Load Custom Script:
The Load Custom Script button will allow you to open a custom script json file generated by the official script tool. This will add a new filter option to the various role selection areas that will only show the roles within that custom script.