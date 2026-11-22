using services.suppliers.Models;

namespace services.suppliers.Validators;

/// <summary>
/// A contract for the shape of a validator that ensures transitioning from one state to another is allowed.
/// </summary>
public interface IStatusTransitionValidator
{
    /// <summary>
    /// Determines if the transition from one state to another is allowed.
    /// </summary>
    /// <param name="currentStatus">The current status to transition from.</param>
    /// <param name="newStatus">The target status to transition to.</param>
    /// <returns>True if the transition is allowed, false otherwise.</returns>
    bool CanTransitionTo(BookingStatus currentStatus, BookingStatus newStatus);
}