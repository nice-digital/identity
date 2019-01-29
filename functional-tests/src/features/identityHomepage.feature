Feature: Guidance list
  As a user of NICE Identity 
  We can access the NICE identity page

  Background:
    Given I open the url "/"

  Scenario: Navigate to Identity homepage
    Given I wait on element ".page-header__heading" to be visible
    Then I expect that element "h1" contains any text
    And I expect that element "h1" contains the text "NICE Identity"
