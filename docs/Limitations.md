
# Limitations and Follow-up Risks

## Current Limitations

1. In-memory storage means data is lost when the
   application stops.

2. Lock provides concurrency protection only within
   one application instance.

3. No persistent database transaction is implemented.

4. The application supports one course offering
   workflow at a time.

5. No authentication or authorization is implemented.

## Future Improvements

1. Introduce SQL Server persistence.

2. Use database transactions and concurrency controls.

3. Introduce REST APIs for external consumers.

4. Add structured logging and monitoring.

5. Add integration tests for persistence.
