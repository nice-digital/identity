Feature: Homepage
  As a user of NICE org 
  I want to be able to use the NICE homepage

  Background:
    Given I have a screen that is 1366 by 768 pixels
      And I open the url "/"

  Scenario: Navigate to find guidance page
    When I click on the link "Find NICE guidance"
    Then I expect that the path is "/guidance"
    And I expect that element "h1" matches the text "Find guidance"
    And I expect that the title is "Guidance | NICE"

  Scenario: Perform a search
    Then I wait on element "[name='q']" to exist
    When I set "test" to the inputfield "[name='q']"
      And I submit the form ".nice-search"
    Then I expect that the path is "/search?q=test"

  Scenario: Use search autocomplete
    Then I wait on element "[name='q']" to exist
    When I set "paracet" to the inputfield "[name='q']"
    Then I wait on element ".tt-suggestions" to exist
      And I expect that element ".tt-suggestion:first-child" matches the text "Paracetamol"
    When I press "ArrowDown"
      And I press "Enter"
    Then I expect that the path is "/Search?q=Paracetamol"

