# RoleBot

A Discord Bot for easily creating roles and letting users manage those roles.

Now written in C# and .NET 8.

[Invite RoleBot to your Discord Server](https://discord.com/api/oauth2/authorize?client_id=740381594669285466&permissions=139855349840&scope=bot%20applications.commands)

[Discord Support Server](https://discord.gg/Za4NAtJJ9v)

![role-bot-20%](https://user-images.githubusercontent.com/4060573/223889561-d4988dcd-9659-4c11-9d2b-d2883087a681.png)

## Important Notes

### Can the bot manage any role?

The bot can only manage roles it has created or roles that have been allowed to be managed by the bot.

### Who can create roles or manage roles?

Only administrators, members with the Manage Roles permission, or members with designated roles can use the bot to create or manage roles. Any user can add or remove their own roles.

## Commands

**Note:** Commands or descriptions below with * can only be used by the administrators

---

`/add-role`

Add a role to yourself.

You can also add a role to another user.*

---

`/allow-role-to-admin`*

Set whether or not the specified role can use the bot to create roles and channels.
NOTE: Only admins or members with designated roles can use the bot to allow roles.

---

`/create-role`*

Create a role.

---

`/list-roles`

List your roles and the available roles (only includes roles that the bot can manage).

---

`/remove-role`

Remove a role from yourself or another user.

---

You can also remove a role from another user.*

---

`/set-existing-role`*

Allow or disallow the bot to manage an existing role.

---

`/set-new-user-role`*

Specify a role that new users will automatically be granted. This can be any role in your server. Leave the role parameter blank to unset this.

---

`/who-has-role`

List the users who have the specified role. Note: currently, roles that are set to not be mentionable cannot be used with this command.

---

`/version`

Get the current version number of the bot.

---
