﻿/*
 * Copyright (c) 2015-2016 EdonGashi
 * https://github.com/EdonGashi/ShipScript
 */

namespace hmV8DynamicNS
{
    public interface IModulePathResolver
    {
        IVirtualPath Resolve(string path, Module parent);
    }
}
