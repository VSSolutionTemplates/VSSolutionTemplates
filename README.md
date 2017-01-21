# VSSolutionTemplates
VSSolutionTemplates are large grained solution accelerators.  They take the form of Visual Studio project templates that generate 
Visual Studio Solutions consisting of multiple integrated sub-projects.  These sub-projects have been specifically designed to 
create a holistic solution baseline that can be used as-is or customized and extended to quickly tailor a given solution to the 
needs of a particular application.  

The difference between VSSoluitonTemplates and more traditional project templates is the aspect of scale. VSSolutionTemplates 
tend to be large grained solutions with multiple integrated subsystems defined as sub-projects and often include ARM templates
that both describe and deploy the end solution.  They are designed to fit together holistically and include built-in 
configuration capabilities that allow for quick and easy customization without requiring solution consumers to understand 
the complexity of the solution for common solution variations.  

Visual Studio Item Templates can also be designed to take advantage of this large scale, holistic understanding of the solution
rather than the project-level scope they are traditionally designed around.  This means that an Item Template can be designed to
take dependencies on other subsystems defined in other projects.  Good design judgement needs to be exercised in these cases but
the acceleration these Item Templates can provide to solution consumers is exponentially greater than the traditional myopic Item
Template. 

At the moment, there is only one solution template ([JumpStreetMobile](https://github.com/VSSolutionTemplates/VSSolutionTemplates/tree/master/VSSolutionTemplates/templates/JumpStreetMobile)) 
but more are being planned.  Stay tuned!

# Footnote
VSSolutionTemplates is built on top of another open source project called [pecan-waffle](https://github.com/ligershark/pecan-waffle).
Pecan-waffle a self-contained command line utility that can be used to easily create and share Visual Studio project templates
and item templates.  It allows you to take an arbitrary Visual Studio project and turn it into a Visual Studio project template
without the need to make any special modifications to the source project.  

In contrast to pecan-waffle, the standard approach for creating project templates requires you to modify the project which breaks
it so you can no long run or test that code base. The whole idea of pecan-waffle is that you can keep you source project in whatever
format you need to develop and test it and then use pecan-waffle to transform the source project into an actual Visual Studio
project template.

While pecan-waffle's approach is a significant benefit for a single project solution, its an absolute necessity for large grained
solutions like VSSoluitonTemplates due to their multiple sub-projects and their tight integration.  As a practical matter, I would
not have been able to build VSSoluitonTemplates without pecan-waffle.

[![Build status](https://ci.appveyor.com/api/projects/status/hjjcd8lj82oeofjs?svg=true)](https://ci.appveyor.com/project/sayedihashimi/vssolutiontemplates)
