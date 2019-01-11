# Front end testing base

> Start point for browser based testing

## What is it?

A starter boilerplate project for writing browser-based functional tests.

We've used a package called [WebdriverIO](http://webdriver.io/) which is a helper library for selenium written in Node.

### Features

- Single command `npm test` to run tests, using [wdio-selenium-standalone-service](https://github.com/webdriverio/wdio-selenium-standalone-service)
- [Cucumber framework](https://cucumber.io/) for writing BDD [features](features)
- Uses [@nice-digital/wdio-cucumber-steps](https://github.com/nhsevidence/wdio-cucumber-steps) shared step definitions
- [TeamCity wdio config](wdio.teamcity.conf.js) with [TeamCity reporter](https://github.com/sullenor/wdio-teamcity-reporter)
- [docker-compose script](run.sh) for running tests against headless Chrome and Firefox in Docker
- [Babel](https://babeljs.io/) for writing custom step definitions in [ES6](https://github.com/lukehoban/es6features#readme) and [Flow](https://flow.org)

## Required software

- Node
- Chrome
- Firefox - optional

## Getting started

- ```git clone https://github.com/nhsevidence/frontend-testing-base.git```
- ```cd frontend-testing-base```
- ```npm i```

After the install has finished run the tests by running the following command. This starts a selenium server and opens Chrome to run the tests:

```sh
npm test
```

> Note: On Windows run in *cmd* and not *GitBash* otherwise the window just hangs.

Optionally, if you've got Firefox installed you can add another [capability](http://webdriver.io/guide/getstarted/configuration.html#desiredCapabilities) in *wdio.conf.js*:

```diff
capabilities: [
+        {
+            browserName: "firefox"
+        }
    ]
```

### Excluding tests

Exclude tests by using the `@pending` [cucumber tag](https://github.com/cucumber/cucumber/wiki/Tags).

### Running single features

To run a single feature file, use the following command:

```sh
npm test -- --spec ./features/homepage.feature
```

Note: you can pass in multiple files, separated by a comma.

Or you can use a keyword to filter e.g.:

```sh
npm test -- --spec homepage
```

Finally, if you've grouped your specs into suites you can run and individual suite with:

```sh
npm test -- --suite homepage
```

See [organizing test suites](http://webdriver.io/guide/testrunner/organizesuite.html) in the WebdriverIO docs for more info.

## Docker

Running tests on Docker is a good option as it means you don't need browsers installed on the host machine, and the Selenium grid is automatically created for you. This is useful on a TeamCity build agent where you can't rely on Chrome and Firefox being installed.

In bash:

```sh
./run.sh
```

Or in CMD on Windows:

```sh
run
```

Or in PowerShell:

```sh
cmd /c "run"
```
