var path = require("path");

exports.config = {

    // Use selenium standalone server so we don't have spawn a separate server
    services: ["selenium-standalone"],
    seleniumLogs: "./logs",

    specs: [
        "./src/features/**/*.feature"
    ],
    exclude: [],

    // Assume user has Chrome and Firefox installed.
    capabilities: [
        {
            maxInstances: 2,
            browserName: "chrome",
            chromeOptions: {
                args: ["--window-size=1366,768"]
            }
        }
    ],

    logLevel: "verbose",
    coloredLogs: true,
    screenshotPath: "./errorShots/",
    baseUrl: "https://www.nice.org.uk/",
    reporters: ["spec"],

    // Use BDD with Cucumber
    framework: "cucumber",
    cucumberOpts: {
        compiler: ["js:babel-register"], // Babel so we can use ES6 in tests
        require: [
            "./src/steps/given.js",
            "./src/steps/when.js",
            "./src/steps/then.js"
        ],
        tagExpression: "not @pending" // See https://docs.cucumber.io/tag-expressions/
    },

    // Set up global asssertion libraries
    before: function before() {
        const chai = require("chai");
        global.expect = chai.expect;
        global.assert = chai.assert;
        global.should = chai.should();
    },
}