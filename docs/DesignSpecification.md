
# Course Registration - Design Specification

## 1. Overview

This application implements course enrollment, dropping
and FIFO waitlist promotion.

The application uses .NET 10 and an in-memory data store.

## 2. Architecture

The solution contains three projects:

- CourseRegistration.Core
- CourseRegistration.Console
- CourseRegistration.Tests

Core contains models, repositories and business services.

Console provides the application entry point.

Tests contains xUnit automated tests.

## 3. Domain Models

### Learner
- Id
- Name
- CompletedCourseIds

### CourseOffering
- Id
- CourseName
- Capacity
- PrerequisiteCourseId

### Enrollment
- LearnerId
- CourseOfferingId
- EnrolledAt

### WaitlistEntry
- LearnerId
- CourseOfferingId
- QueuePosition
- AddedAt

## 4. Service Boundary

IEnrollmentService exposes:

- Enroll(learnerId, courseOfferingId)
- Drop(learnerId, courseOfferingId)

EnrollmentService implements the business rules.

## 5. Business Rules

1. A learner can enroll only once per course offering.
2. Prerequisites must be completed before enrollment.
3. If seats are available, enrollment succeeds.
4. If the course is full, eligible learners join the waitlist.
5. Duplicate waitlist entries are rejected.
6. Dropping an enrolled learner opens a seat.
7. The earliest eligible waitlisted learner is promoted.
8. Ineligible waitlisted learners are skipped and removed.
9. Invalid operations return meaningful status results.

## 6. Capacity Invariant

The number of enrolled learners must never exceed
the course capacity.

All enrollment, drop and promotion operations use
a shared lock to protect in-memory state.

## 7. Waitlist Strategy

FIFO (First In, First Out) is implemented using
QueuePosition.

The lowest queue position is considered first.

If a learner is no longer eligible, the learner is
removed and the next eligible learner is considered.

## 8. Concurrency

A shared lock protects state-changing operations.

This prevents concurrent requests from exceeding
the course capacity within this application instance.

## 9. Persistence

The current implementation uses in-memory collections.

Data is not persisted after application termination.

## 10. Testing

xUnit is used for automated testing.

Tests cover enrollment, capacity, duplicate requests,
prerequisites, FIFO promotion, invalid drops and concurrency.
