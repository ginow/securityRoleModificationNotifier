# CRM Security Role addition or removal notifier

## Requirement
Whenever some addition or removal of security roles is done on some user's profile an email notification should be sent to a particular person with the details of change.

## Implementation
In CRM systems whenever a role is added or removed "Associate" and "Disassociate" plugin messages are triggered. These messages are not only triggered for roles, but for other entities as well.
A plugin was registered on both these messages. It is then used to retrieve security roles, user and then updates another custom entity with the name "flowemail". Using Microsoft Flow an email will be sent whenever this custom entity is updated, with the email body containing the details of change.

### Screenshot of plugin registration tool
Note: Primary and secondary entity were set as empty
![Plugin tool screenshot](https://github.com/ginow/securityRoleModificationNotifier/blob/master/tool.png)

### Screenshot of flow
Note: Here Gmail was used, others can also be used.
![Flow screenshot](https://github.com/ginow/securityRoleModificationNotifier/blob/master/flow.png)

