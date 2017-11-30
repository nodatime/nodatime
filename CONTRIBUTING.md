# How to Contribute

Thanks for your interest in Noda Time. We appreciate all kinds of contributions, from submitting issues to improving documentation; from writing tests to implementing new code. All help is welcome!

## Basic Requirements

If you want to contribute to the codebase, you're going to need a text editor or IDE. We recommend [Visual Studio Community 2017](https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx)(Windows) or [Visual Studio Code](https://code.visualstudio.com/)(Windows/Linux/macOS).

You're also going to need .NET Core SDK installed - you can download it from [here.](https://www.microsoft.com/net/download/)

Please make sure you have a [git client](https://git-scm.com/) installed. If you don't already have a GitHub account, [please create one.](https://github.com/signup/free)

After you're all set, you can [fork the project](https://help.github.com/articles/fork-a-repo). Then you'll be able to clone your fork, so you can edit the files locally on your machine:

```Text
git clone https://github.com/YOUR-USERNAME/nodatime.git
```

Once you clone the repository, you'll have a [remote repository](https://git-scm.com/book/en/v2/Git-Basics-Working-with-Remotes) (or simply *remote*) called `origin`, that points to your forked repository on GitHub.

You'll usually want to add another remote, pointing to the original repository on GitHub. It's an acepted convention to call this remote *upstream*. You can do it like this:

```Text
git remote add upstream https://github.com/nodatime/nodatime.git
```

## How to start contributing?

We have a [`help-wanted`](https://github.com/nodatime/nodatime/labels/help%20wanted)
label on our issue tracker to indicate tasks which new contributors can work on without much previous experience in Noda Time.

If you've found something you'd like to help with, please leave a comment in the issue.

Additionally, feel free to open an issue if you find a bug or want to suggest a feature or enhancement.

### Making Changes

When you're ready to start working, create a new branch off the `master` branch:

```
git checkout master
git pull upstream master
git checkout -b SOME-BRANCH-NAME
```

Try to use a short, descriptive name for your branch, such as `add-tests-foobar-struct`.

### Building

To build everything under Visual Studio, simply open the src/NodaTime-All.sln solution file and build normally. To build with just the .NET Core SDK, run

```Text 
dotnet build src/NodaTime-All.sln
```

### Running Tests

The tests are currently console application projects. Simply run the following commands:

```Text
cd src/NodaTime.Test
dotnet run -f net451
dotnet run -f netcoreapp1.0
```

### Submitting Changes

To publish your branch to your local fork, run this command from the Git Shell:

```Text
git push origin -u MY-BRANCH-NAME
```

When your work is finished, [open a pull request](https://help.github.com/articles/using-pull-requests) against your changes.

If your pull request fixes an issue, add a comment with the word "Fixes", "Resolves" or "Closes", followed by the issue's number:

>   Fixes #1145

If you need to, feel free to add comments to the PR asking for suggestions or help.
