# Подключение библиотеки к проекту с Source Generator
см. https://www.youtube.com/watch?v=wp-dxZXRkJ4

Ниже приведена конфигурация `.csproj`, которая нужна для того чтобы использовать подключаемые библиотеки
в Source Generator'ах. В данном примере подключается библиотека `Inflector.NetStandard`
```xml
<Project Sdk="Microsoft.NET.Sdk">
    <!-- ...дефолтная конфигурация проекта... -->

    <!-- Библиотеки: -->
    <ItemGroup>
        <!-- ...другие библиотеки... -->
        <!-- Установленна библиотека должна иметь ключи: PrivateAssets="all" GeneratePathProperty="true" -->
        <PackageReference Include="Inflector.NetStandard" Version="1.2.2" PrivateAssets="all"
                          GeneratePathProperty="true"/>
    </ItemGroup>

    <!-- Это нужно чтобы к дефолтному пути добавить свой, который определен ниже -->
    <PropertyGroup>
        <GetTargetPathDependsOn>$(GetTargetPathDependsOn);GetDependencyTargetPaths</GetTargetPathDependsOn>
    </PropertyGroup>

    <!-- Здесь указывается путь к DLL файлу установленной библиотеки -->
    <Target Name="GetDependencyTargetPaths">
        <ItemGroup>
            <TargetPathWithTargetPlatformMoniker Include="$(PKGinflector_netstandard)\lib\netstandard2.0\Inflector.dll"
                                                 IncludeRuntimeDependency="false"/>
            <!-- Pack both our DLL and the Dependency DLL into a Generated Nuget Package -->
            <None Include="$(OutputPath)\$(AssemblyName).dll" Pack="true" PackagePath="analyzers/dotnet/cs"
                  Visible="false"/>
            <None Include="$(Pkginflector_netstandard)\lib\netstandard2.0\*.dll" Pack="true"
                  PackagePath="analyzers/dotnet/cs" Visible="false"/>
        </ItemGroup>

        <!-- Сюда можно добавлять другие библиотеки -->
    </Target>
</Project>
```

Для того чтобы установленная через nuget библиотека работала в Source Generator'ах, для генератора
нужно указать путь где брать DLL этой библиотеки.

**Пример:**

Библиотека: `Inflector.NetStandard`

Путь к DLL`$(PKGinflector_netstandard)\lib\netstandard2.0\Inflector.dll`,
где `$(PKGinflector_netstandard)` переменная для доступа к папке библиотеки, `PKG` - стандатный
префикс, который будет использоваться всегда `inflector_netstandard` папка в которой установлена библиотека.
Если в названии папки есть точки, они заменяются на нижнее подчеркивание.

Чтобы узнать название папки нужно на ПК перейти в папку куда устанавливаются Nuget пакеты, обычно это
папка `<пользователь>/.nuget/packages` и найти нужный пакет.

Папка данной библиотеки называется: `inflector.netstandard`, точка заменяется на нижнее подчеркивание,
и в начало прибавляется префикс `PKG`, т.е. получается следующая часть пути `$(PKGinflector_netstandard)`.

После чего нужно зайти в папку `inflector.netstandard` и перейти в папку установленной версии, в данном случае
в папку `1.2.2`, после чего найти `DLL` подходящую под данный проект. Все `DLL` находятся в папке `lib`.
Так как проект `netstandart2.0`, то и `DLL` должна быть версии для `netstandart2.0`.
В папке `lib/` нужно найти папку `netstandart2.0` и в ней `DLL` этой библиотеки.

В данном случае `DLL` назвывается `Inflector.dll`, т.е. путь к этой `DLL` следующий:
`\lib\netstandard2.0\Inflector.dll`


`<None Include="$(Pkginflector_netstandard)\lib\netstandard2.0\*.dll" Pack="true"
PackagePath="analyzers/dotnet/cs" Visible="false"/>` указывает на то, что все `DLL` в `lib/netstandard2.0`
подключаемой библиотеки нужно добавить в билд.

### При добавлении новых библиотек нужно:
1. В `PackageReference` добавить `PrivateAssets="all" GeneratePathProperty="true"`
2. В ` <Target Name="GetDependencyTargetPaths">` добавить 
   `<TargetPathWithTargetPlatformMoniker Include="$(PKGinflector_netstandard)\lib\netstandard2.0\Inflector.dll" IncludeRuntimeDependency="false"/>`
    заменив значение в `Include` на подключаемую библиотеку
3. В ` <Target Name="GetDependencyTargetPaths">` добавить 
   `<None Include="$(Pkginflector_netstandard)\lib\netstandard2.0\*.dll" Pack="true" PackagePath="analyzers/dotnet/cs" Visible="false"/>`
    заменив значение в `Include` на подключаемую библиотеку