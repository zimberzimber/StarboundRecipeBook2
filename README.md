# Starbound Recipe Book 2
A project aiming to create a place where users could view all the items and recipes in once concentrated space, serving sort of like a wiki, but just for items.
As a side effect, modders will have to answer the same item-recipe related questions less.

### Frequently Asked Questions ###
"Why 2?"
> Because the first recipe book was planned as a mod for Starbound instead of a web app, but there was no implementable way without either dropping extra work on every mod author out there, or endless hours hours on my end.

"I want an item added by mod hidden."
> Add the following line to the items you want hidden:
> "SBRBhidden" : true
> The items entry will not be hidden, but its internal name, unlocks, and involved recipes, will not be displayed.

"I want my mod off the book."
> Message me, and we'll talk about it.

## Features ##
- A database containing items (including consumeables, active items, and object), created by reading an extracted mod

## To Do ##
- A GUI to view the items
- Include recipes
- A GUI to view the recipes
- Include patches the way SB does
- Option to view specific mods
- Option to change mod order
- Option to download a file containing all the required data based on selected mods and their order for a functional in-game recipe book
