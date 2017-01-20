# VSSolutionTemplates
VSSolutionTemplates are large grained solution accelerators.  They take the form of Visual Studio project templates that generate Visual Studio Solutions consisting of multiple integrated sub-projects.  These sub-projects have been specifically designed to create a holistic solution baseline that can be used as-is or customized and extended to quickly tailor a given solution to the needs of a particular application.  

The difference between VSSoluitonTemplates and more traditional project templates is the aspect of scale. VSSolutionTemplates tend to be large grained solutions with multiple integrated subsystems defined as sub-projects and often include ARM templates that both describe and deploy the end solution.  They are designed to fit together holistically and include built-in configuration capabilities that allow for quick and easy customization without requiring solution consumers to understand the complexity of the solution for common solution variations.  

Visual Studio Item Templates can also be designed to take advantage of this large scale, holistic understanding of the solution rather than the project-level scope they are triditionally designed around.

Initially, there is only one solution template (JumpStreetMobile) but more are being planned.  Stay tuned!

[![Build status](https://ci.appveyor.com/api/projects/status/hjjcd8lj82oeofjs?svg=true)](https://ci.appveyor.com/project/sayedihashimi/vssolutiontemplates)
