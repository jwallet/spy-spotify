using System;
using System.Runtime.InteropServices;

namespace EspionSpotify.Native
{
    /// <summary>
    /// Provides interoperability for "combase.dll".
    /// </summary>
    public static class Combase
    {
        /// <summary>
        /// Gets the activation factory for the specified runtime class.
        /// </summary>
        /// <param name="activatableClassId">The ID of the activatable class.</param>
        /// <param name="iid">The reference ID of the interface.</param>
        /// <param name="factory">The activation factory.</param>
        [DllImport("combase.dll", PreserveSig = false)]
        public static extern void RoGetActivationFactory(
            [MarshalAs(UnmanagedType.HString)] string activatableClassId,
            [In] ref Guid iid,
            [Out, MarshalAs(UnmanagedType.IInspectable)] out object factory);

        /// <summary>
        /// Creates a new HSTRING based on the specified source string.
        /// </summary>
        /// <param name="src">A null-terminated string to use as the source for the new HSTRING. To create a new, empty, or NULL string, pass NULL for sourceString and 0 for length.</param>
        /// <param name="length">The length of sourceString, in Unicode characters. Must be 0 if sourceString is NULL.</param>
        /// <param name="hstring">A pointer to the newly created HSTRING, or NULL if an error occurs. Any existing content in string is overwritten. The HSTRING is a standard handle type..</param>
        [DllImport("combase.dll", PreserveSig = false)]
        public static extern void WindowsCreateString(
            [MarshalAs(UnmanagedType.LPWStr)] string src,
            [In] uint length,
            [Out] out IntPtr hstring);
    }
}