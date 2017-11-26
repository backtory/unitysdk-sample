Change Log
==========
## Version 0.6 *(1396/7/30)*
* New: **In App Purchase!** Easily and securely purchase and consumes items from Cafe Bazaar.
  See documents for setup guide. 
* New: **Sample query.** Get random records matched to a query constrains.
* New: Ability to add custom fields (limited to _string_ type) to `BacktoryUser` with `Put` method. 
* New: You can now pass `true` to `RealtimeGame.SendGameEvent` method's second parameter to  
  to ignore webhook call for this particular event.
* Fix: Websocket not emptying queue before stop.
* Fix: 404 on logouts.
   

### Version 0.5.13 *(1396/7/23)*
* New: **Count query.** Get just the number of rows matching the query.
* New: Columns returned by a query could be limited using BacktoryQuery `Select` method. This helps using less traffic when all 
 columns of records are not needed.

### Version 0.5.12 *(1396/7/17)*
* Fix: Getting exception when performing an empty query (`SELECT *` equivalent).
* Fix: MetaData and challengeName parameters in `BacktoryChallenge.CreateNew` getting wrong value.
* Change: `BacktoryMatch` is now public.
* New: `BacktoryChallenge.ActivateChallenge`. Along with `BacktoryMatch` being public, now you can
  manually start a challenge even if minimum number of required players.
* Change: Throwing exception on using all of services without a logged-in user.
* Change: Throwing exception on using all of services without their instance id. 

### Version 0.5.9 *(1396/5/20)*
* Fix: Not showing error messages.
* Fix: Not throwing exception using realtime and object storage without access-token.
* Fix: Not passing ExtraMessage to MatchUpdatedListener.

### Version 0.5.8 *(1396/3/18)*
* New: Atomic Increment in `BacktoryObject`
* Minor bug fixes, better error handling and clean-ups 

### Version 0.5.3 *(1396/3/18)*
* New: Easily switch between dev and production server from editor inspector.
* New: Global chat room. Let users chat in predefined global rooms.

## Version 0.5.0 *(1396/3/3)*
* New: Add nested lists and dictionaries to `BacktoryObject` and get them from retrieved ones
* New: Forgot password. Let your users recover their account if they've forgotten their passwords. 
* New: Revert a `BacktoryObject` changes to server state using `Revert` methods.

### Version 0.5.0-beta5 *(1396/2/14)*
* Fix: __Getting disconnected from game after a few minutes.__
* New: __SDK version and debug-mode in setting inspector.__
* Incompatible change: Backtory Setting inspector is now included in `Backtory.dll`. 
  __You must remove `Backtory -> Editor` folder and re-enter your keys__. 
* Incompatible change: BacktoryMatch now contains list of `BacktoryMatchParticipant` instead of 
  list of string.
* Fix: Empty event when using `BacktoryGameEvent`.
* Fix: OnMatchmakingUpdate not getting called
* New: Use `BacktoryClient.IsRealtimeServiceConnected` to see if Realtime is connected and 
  `BacktoryRealtimeGame.State` for knowing in which state of the game 
  the `BacktoryUser.CurrentUser` is.  

### Version 0.5.0-beta1 *(1396/1/15)*
* New: __Introducing Database!__ Save your objects using BacktoryObject and retrieve 
  them by BacktoryQuery classes. Note that database in SDK is limited to saving simple objects
  (no relation and only primitive types), updating, deleting and retrieving them using query 
  constraints.
* Fix: userId null when getting CurrentUser after relaunching.
* Incompatible change : BacktoryUser.GetCurrentUser() now is BacktoryUser.CurrentUser 
* New: UserId is now public. Access it by BacktoryUser.CurrentUser.UserId
* Incompatible change : Some renaming in Realtime classes.

## Version 0.4 *(1395/11/28)*
* New: __File Storage!__ You can upload/rename/delete files to/in/from Backtory.

## Version 0.3 *(1395/11/21)*
* New : __Realtime is back with new design!__ Refer to documentations to see how the new API works.
* Incompatible change : Login and Login as guest responses no longer have a body, so you've no access to user access token

#### Version 0.2.2.3 *(1395/10/9)*
 * Fix: __Getting NullReferenceException on timed-out request__
 * enhancement: removing stackTraceLogType. It's now up to unity developers. 

#### Version 0.2.2.1 *(1395/10/1)*
 * Fix: SDK no longer needs Unity 5.4 or newer

### Version 0.2.2 
 * Incompatible change	: __Setting file (for storing user login data) is renamed, so you must force user to login again.__
 * Fix : __Storage in android no longer fails__ 
 * Fix : default value handling in json serialization. (0, false, null now included in json string)
 * Fix : Not getting timeout sometimes by setting default read and write timeout to 10 seconds
 * New : Debug mode! By starting SDK in debug mode you can see SDK log statements
		containing data about SDK operations (e.g. network request and responses)
 * enhancement : single DLL dependency, adding XML doc
## Version 0.2
 * Incompatible change : __Removing realtime entirely!__ Realtime needed a lot of improvements and fixes so we decided to rewrite it.
 Realtime will be added to SDK again in version 0.3 
 * Incompatible change : All of SDK services now return IBacktoryResponse instead of BacktoryResponse
 * Incompatible change : Renamed namespaces. You should not use any class
		from internal namespaces.

### Version 0.1.1
* Typo : renaming sendAsync in BacktoryGameEvent to SendInBackground
* Typo : correcting some XML doc Typos
* bug fix : correcting unity singleton mechanism (used to destroy on changing scene)
* Typo : wrong address in editor config. Changing "Backtory/Resources" to BacktorySDK/Resources". Also replacing GameSparks with Backtory in constant names! :)))
* New : realtime! Also cleaning folder structure after merging realtime to dev

