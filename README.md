Azure SignalR UserGroups Issue Repo 
=================================

## Run the sample

### Start the negotiation server

```
cd NegotiationServer
dotnet user-secrets set Azure:SignalR:ConnectionString "<Connection String>"
dotnet run
```

### Start SignalR clients

```
cd SignalRClient
dotnet run
```

### Start message publisher

```
cd MessagePublisher
dotnet run
```

Once the message publisher starts, execute the following command and see that the user DOES get the message

```powershell
usergroup check User group1 
# You should see: check is user 'User' in group 'group1': True
send group group1 Test
# SignalRClient should log: User: gets message from service: 'test'
```

### Muck with the groups to cause issues

Navigate to https://localhost:5001/test which will remove the user from groups and add them to a number of groups.

### Try to send messages again

```powershell
usergroup check User group1 
# You should see: check is user 'User' in group 'group1': True
send group group1 Test
# SignalRClient shouldn't log anything
```