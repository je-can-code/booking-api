namespace services.suppliers.Models;

public enum BookingStatus
{
    /// <summary>
    /// The initial state before any changes take place by the supplier.
    /// </summary>
    RECEIVED = 0,
    
    /// <summary>
    /// The supplier didn't reply before the timeout, so we wait.
    /// </summary>
    PENDING,
    
    /// <summary>
    /// The supplier replied and gave us a supplierReferenceId! :)
    /// </summary>
    CONFIRMED,
    
    /// <summary>
    /// The supplier replied and told us they aren't available to serve the request. :(
    /// </summary>
    REJECTED,
    
    /// <summary>
    /// The customer decided they didn't want the booking after all. :(
    /// </summary>
    CANCELLED,
    
    /// <summary>
    /// A fallback status for when the booking status is otherwise in limbo. :?
    /// </summary>
    UNKNOWN,
}