using services.suppliers.Models;

namespace services.suppliers.Validators;

internal sealed class StatusTransitionValidator : IStatusTransitionValidator
{
    public bool CanTransitionTo(BookingStatus currentStatus, BookingStatus newStatus)
    {
        // no reason to transition to same-state.
        if (newStatus == currentStatus) return false;
        
        switch (currentStatus)
        {
            // cannot reverse a pending status.
            case BookingStatus.PENDING when newStatus == BookingStatus.RECEIVED:

            // cannot reverse from confirmed/rejected into a less-sure status.
            case BookingStatus.CONFIRMED or BookingStatus.REJECTED when
                newStatus is BookingStatus.RECEIVED or BookingStatus.PENDING:
            
            // cannot cycle between the various terminal states.
            case BookingStatus.CONFIRMED when newStatus is BookingStatus.REJECTED:
            case BookingStatus.REJECTED when newStatus is BookingStatus.CONFIRMED or BookingStatus.CANCELLED:
                
            // cannot un-cancel a booking.
            case BookingStatus.CANCELLED when newStatus is not BookingStatus.UNKNOWN:
                return false;

            // note that UNKNOWN is not factored in as an allowed to or from transition state.
            
            // transition allowed!
            default:
                return true;
        }
    }
}