// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Autofac;

namespace ReflectionEventing.Autofac;

public class AutofacEventBusBuilder(ContainerBuilder builder) : EventBusBuilder;
