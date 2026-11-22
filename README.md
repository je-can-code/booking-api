# Booking API

A service for managing bookings between suppliers and distributors.

## Setting it up

Pull the codebase and open the solution in your IDE of choice to review the code. For convenience, it is recommended to use your IDE's means to run the application (usually, the "play" button). If you like the CLI, use this from the root folder:
```bash
dotnet run --project src/api --launch-profile https
```

To run the tests, again your IDE can be a convenient answer, but the CLI works just as well:
```bash
dotnet test
```

## Tech Stack

This application was written using C#, using the dotnet 10 SDK. It leverages the standard ASP Webapi template, and leverages SQLite for its database.

## API descriptions

This API contains three endpoints (suggested by the assignment):

> NOTE: for testing purposes with the mock luxurio api, if `roomTypeId` contains the words "accepted" or "reject", the logic will auto-confirm or auto-reject. This can be used to skip the timeout and need to hit the `POST /Suppliers/callbacks` endpoint for follow-up. Again, this is for testing purposes and wouldn't be implemented in a real API.

```
POST    /Bookings
GET     /Bookings/:bookingId
POST    /Suppliers/callbacks
```

### `POST /Bookings`

This endpoint is the "create" endpoint for creating a new booking record. It is expected that distributors will be the primary user for this endpoint.

- 200: when successful or when a booking already exists with the same `distributorId` and `idempotencyKey`. It returns the booking metadata.
- 400: when any required information is missing. It returns standard problem details for the caller.
- 500: any unexpected issue that happens in the server. It returns the standard problem details for the caller.

#### Request

An example of one such request might look like this:

```bash
POST https://localhost:7108/Bookings
```

```json
{
  "idempotencyKey": "qwertybox",
  "distributorId": "super-distro-123",
  "propertyId": "magna-carta-prestige",
  "roomTypeId": "luxurious-9001",
  "checkIn": "2026-01-16",
  "checkOut": "2026-01-21",
  "guest": {
    "firstName": "jeremy",
    "lastName": "espinoza"
  }
}
```

#### Response

```json
{
  "data": {
    "bookingId": "69499ca5-9c40-4b2d-aa44-d3ea0eea9e62",
    "supplierReferenceId": "",
    "status": "PENDING",
    "createdAt": "2026-11-21T22:48:55.7242712+00:00",
    "updatedAt": "2026-11-21T22:49:38.3806677+00:00"
  }
}
```

### `GET /Bookings/:bookingId`

This endpoint is the "read" endpoint for getting an existing booking record. It is expected that potentially any
frontend that might care about this data would be hitting this endpoint.

- 200: when the record exists. It returns the booking metadata.
- 404: when the record does NOT exist or the `bookingId` isn't `Guid`-shaped. It returns a standard 404 not found response.
- 500: any unexpected issue that happens in the server. It returns the standard problem details for the caller.

#### Request

```bash
GET https://localhost:7108/Bookings/69499ca5-9c40-4b2d-aa44-d3ea0eea9e62
```

#### Response

```json
{
  "data": {
    "bookingId": "69499ca5-9c40-4b2d-aa44-d3ea0eea9e62",
    "supplierReferenceId": "luxurio-supreme-is-the-best",
    "status": "CONFIRMED",
    "createdAt": "2026-11-21T22:48:55.7242712+00:00",
    "updatedAt": "2026-11-21T22:50:50.7747972+00:00"
  }
}
```

### `POST /Suppliers/callbacks`

This endpoint is the "update" endpoint (loosely) for suppliers to reply yay/nay regarding a booking record. It is expected that only suppliers would call this endpoint.

- 200: when the record's status was updated successfully- or when they tried to update to the same status again (ex: confirmed -> confirmed).
- 400: when any required information is missing. It returns standard problem details for the caller.
- 404: when the record does NOT exist. It returns a standard 404 not found response.
- 500: any unexpected issue that happens in the server. It returns the standard problem details for the caller.

#### Request

```json
{
  "bookingId": "69499ca5-9c40-4b2d-aa44-d3ea0eea9e62",
  "supplierReferenceId": "luxurio-supreme-is-the-best",
  "status": "CONFIRMED"
}
```

#### Response

```json
{
  "data": {
    "bookingId": "69499ca5-9c40-4b2d-aa44-d3ea0eea9e62",
    "supplierReferenceId": "luxurio-supreme-is-the-best",
    "previousStatus": "PENDING",
    "newStatus": "CONFIRMED"
  }
}
```

## Assumptions

- All calls out to a supplier are captured and stored on their end, we do not store the stay information on our side at all.
- All duplicate calls to `POST /Bookings` are retries.

## Known Limitations

- there isn't as much validation as I'd like. Something like `checkIn` being before `checkOut` is among the things I'd like to have implemented.
- no form of "reconciliation" in this app at all.
- no auth (anyone hitting the `POST /Suppliers/callbacks` endpoint can change status!!!)
- if `StayInfo` needs to be persisted, its not. I opted against it entirely, but maybe thats something we need.
- the `POST /Suppliers/callbacks` will take ANY status- it probably shouldn't, but I'd normally lean on a product person to decide that.

# Architecture

The overall API was designed using layers to separate where the code lives logically.

## Major Components

Well, I built the app in layers:
- the `api` layer: where we have the two controllers, one for the booking CR (no UD) layer and one for the suppliers U (no CR or D lol).
- the `services` layer: where we have domain/business logic, and do most the orchestration
- the `data` layer: where we isolate our database functionality. 

> Then of course, 1 test project per layer listed above.

## Booking Request Flow

The expected flow based on how I built it and what I understand of the general process:
- distributor calls `POST /Bookings` with a new booking
- request is mapped and flows out to the supplier ("LuxurioSupreme") where it waits a bit.
  - the supplier can respond with confirm.
  - the supplier can respond with reject.
  - the supplier doesn't respond at all, and it times out.
- the response is mapped and returned to the caller.
- the caller can use `GET /Bookings/:bookingId` to check the status anytime.
- the supplier can use `POST /Suppliers/callbacks` to confirm/reject the booking anytime.

Additionally, there is validation logic to protect "expected" flow of booking status to prevent doing something like taking a booking from REJECTED → RECEIVED.

So to define the flow of `BookingStatus` aka booking states, it looks something like this:

1) RECEIVED (we received your request for a booking- the initial state)
2) PENDING (we reached out to a supplier, but alas the request timed out- they will use the `POST /Suppliers/callbacks` endpoint to follow-up)
3) CONFIRMED/REJECTED (the supplier reached back and hit the `POST /Suppliers/callbacks` endpoint with a response! These are terminal states.)
4) CANCELLED (the customer ctrl+z'd their booking, or the supplier received word the request was canceled. This is a terminal state.)
5) UNKNOWN (this code does not set UNKNOWN explicitly, but if the supplier perhaps had some extenuating circumstances, or later we implement logic that may throw the booking into this state, its available. This state can be entered into from any state, and can exit into any state, so it is not to be used lightly!)

## Timeout and Late-confirmation Handling

The "timeout" handling is when my booking service decides to timeout (after the mock api waits 3 seconds because you didn't give it the magic `roomTypeId` word), and ultimately throws an exception. We catch that and assume "okay, supplier got the request, its PENDING now".

It also assumes that "if there is a timeout, then there will eventually be a supplier callback with reject/confirm"- the `POST /Suppliers/callbacks` endpoint.

## Idempotency Approach

If the caller attempts to create a booking with a `distributorId` and `idempotencyKey` that already exists on a record in the database, it will instead assume it's a retry and return the existing booking back to the caller. This is locked down via a unique index setup against `(distributorId, idempotencyKey)` and is handled with try-catch in the `BookingService` implementation with the `catch (DbUpdateException ex)` logic then following up with a check against the index values.

If the caller attempts to update the status of a booking with the same status, it will log the attempt and just return the same shape of response stating the before/after status are the same.

## Persistence Strategy

For the purpose of this take-home assignment, I chose to use SQLite. Only the booking metadata is persisted, while the actual stay information is handed off to the supplier.

## Retry or Reconciliation Strategy

If the caller attempts to recreate an existing booking, it will be blocked (assumes it is a retry).

## Important Trade-offs

Hmm, this is always a challenging section because I like to think all the choices I made are the right ones? But here is what I've got:

- I COULD have created a more complex "timeout" scenario with async background workers handling a reply or something, but opted for a plain `WaitAsync` and an arbitrary synchronous timeout if you don't use "the magic `roomTypeId`s", followed by try-catch handling instead.
- I COULD have used postgres- which would probably be my goto in a production application, but opted for SQLite considering this is a take-home test and not a uhh, production app.
- I COULD have checked for duplicates ahead of creation and gone hard with database locking and such, but opted instead to lean on try-catch exception handling for readability to deal with it.
- I COULD have used a real HttpClient and built a real scenario with retries and such, but I instead opted to mock the entire functionality and fake it with alternative logic for simplicity.

# Architecture Diagram

Below is a crude textual flow-like diagram of the architecture of this API.

- distributor -> `BookingsController` -> `BookingService` -> `BookingRepository` -> DB (SQLite)
- supplier -> `SuppliersController` -> `BookingService` -> `BookingRepository` -> DB (SQLite)
- `BookingService`
  - -> `StatusTransitionValidator`
  - -> our "supplier" `MockLuxurioSupremeApi`

# Production Design Questions

## 1. Supplier Timeout

Q: The supplier request times out, but it may still have created the booking. What status should KBKx return to the distributor, and what should happen next?

A: The implementation will write the status of that booking to be "PENDING" and return the booking response like normal. It is expected the supplier will follow-up with a response.

## 2. Distributor Retry

Q: The distributor retries the request with the same idempotency key. What response should the service return?

A: The implementation will respond with the previous booking they created. The idempotency key must be distinct per booking or its treated as a retry.

## 3. Late Confirmation

Q: The supplier confirms the booking several minutes after the original request timed out. How should the service process and expose that change?

A: The implementation will handle the `POST /Suppliers/callbacks` call with an update to the database record. The next `GET /Bookings/:bookingId` will yield the updated status and also contain the `supplierReferenceId`.

## 4. Service Restart

Q: How would the design ensure that pending or unknown bookings are not lost after an application restart?

A: The database is not the same as the service. As long as the database persists, no bookings of any kind will be lost on application restart, so make sure the database is not an in-memory database (this assignment directed towards this).

## 5. Multiple Service Instances

Q: What would change if several instances of the booking service were processing requests concurrently?

A: The database access is the biggest thing that would need to change as SQLite would maintain separate instances of the database.

## 6. Production Scale

Q: What components or patterns would you consider if the service handled millions of bookings across hundreds of
suppliers?

A: Horizontal scaling of multiple application instances and database sharding w/ replication and caching would probably be first on my list to tackle. Additionally, eventual consistency for a database may be a better approach rather than an always-up-to-date database strategy. 

# Time Management

## Work Completed Within 4 Hours

I was able to get some of it done within four hours:

- hour1: took time to digest the assignment and sort out an approach
- hour2: took time making sure dotnet was set up on my computer and worked (I use Linux, and didn't have it setup on my computer)
- hour3: laid out the layers of the application and test projects and other basic plumbing
- hour4: got some of the logic in-place, like the endpoints reachable and contracts sorted out

## But I kept going for a few more hours

I wasn't done yet, and spent extra time to make sure:

- the implementation was at least feature complete.
- there was some small amount of logging in-place.
- there were tests in place, including a full-flow e2e test.
- this document was filled out! 

Honestly, it has been a while since I've had to spin up a whole dotnet project from cold without a template or shared libraries or anything- but it was a fun adventure and I didn't want to give you anything half-baked! I look forward to talking with you about this!