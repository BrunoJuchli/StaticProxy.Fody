using System.Runtime.InteropServices;
using Xunit;

// disable test parallelization - tests need (exclusive) access to binaries
[assembly: CollectionBehavior(DisableTestParallelization = true)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("15710acc-93a1-4882-bda1-27b2e14e7f6d")]
