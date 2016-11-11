param($installPath, $toolsPath, $package, $project)

$project.ProjectItems.Item("libmp3lame.32.dll").Properties.Item("CopyToOutputDirectory").Value = 1
$project.ProjectItems.Item("libmp3lame.64.dll").Properties.Item("CopyToOutputDirectory").Value = 1
