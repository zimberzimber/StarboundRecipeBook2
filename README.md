# Starbound Recipe Book 2
A project aiming to create a place where users could view all the items and recipes in once concentrated space, serving sort of like a wiki, but just for items.
As a side effect, modders will have to answer the same item-recipe related questions less.

Come say hi! <br/>
<a href="https://discord.gg/Egjx962">
<img src="https://discordapp.com/assets/e4923594e694a21542a489471ecffa50.svg" alt="Discord Link" width="200" height="68">
</a>

## Common Queries ##

**I want a mod added to the list.**
> Message me the Steam ID of the subject mod on the Discord server or via Steam, and I'll work on adding it.

**I want an item added by my mod hidden.**
> Add the following line to the items you want hidden:<br/>
> "SBRBhidden" : true<br/>
> The items entry will not be hidden, but its internal name, unlocks, and involved recipes, will not be displayed.

**I want my mod off the book.**
> Message me, and we'll talk about it.

**Whats used in the project?**
> ASP.NET Core by Microsoft - Handling the web app<br/>
> Entity Framework Core by Microsoft - Handling the database<br/>
> JIL by Kevin Montrose - Handling JSON de/serialization : https://github.com/kevin-montrose/Jil<br/>
> ImageSharp by Six Labors - Handing image conversion : https://sixlabors.com/projects/imagesharp/

**Why 2?**
> Because the first recipe book was planned as a mod for Starbound instead of a web app, but there was no implementable way without either dropping extra work on every mod author out there, or endless hours hours on my end.

**Common Queries?**
> Yes, because most of these aren't questions."

## Features ##
- [x] **A database containing items (including consumeables, active items, and object), created by reading an extracted mod**
- [ ] A GUI to view the items
- [ ] Include recipes
- [ ] A GUI to view the recipes
- [ ] Include patches the way SB does
- [ ] Option to view specific mods
- [ ] Option to change mod order
- [ ] Option to download a file containing all the required data based on selected mods and their order for a functional in-game recipe book
