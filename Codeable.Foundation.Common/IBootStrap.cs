﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeable.Foundation.Common
{
    /// <summary>
    /// Supports pro-active registration (instead of dynamic registration)
    /// </summary>
    public interface IBootStrap
    {
        /// <summary>
        /// The current chain of IBootStrap
        /// </summary>
        List<IBootStrap> BootStrapChain { get; set; }

        /// <summary>
        /// Occurs when the foundation has been created. Before any Registers have been executed.
        /// </summary>
        void OnFoundationCreated(IFoundation foundation);

        /// <summary>
        /// Occurs before self registers are executed
        /// </summary>
        void OnBeforeSelfRegisters(IFoundation foundation);
        /// <summary>
        /// Occurs for every executing self register. 
        /// </summary>
        void OnSelfRegister(IFoundation foundation, SelfRegisteringArgs args);
        /// <summary>
        /// Occurs after all self registers have been registered
        /// </summary>
        void OnAfterSelfRegisters(IFoundation foundation);

        /// <summary>
        /// Occurs when the primary bootstrap has completed.
        /// </summary>
        void OnBootStrapComplete(IFoundation foundation);
        /// <summary>
        /// Occurs after the bootstrap has completed, immediately before leaving the bootstrap.
        /// </summary>
        /// <param name="foundation"></param>
        void OnAfterBootStrapComplete(IFoundation foundation);

    }
}
