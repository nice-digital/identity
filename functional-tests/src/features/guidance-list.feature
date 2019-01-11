Feature: Guidance list
  As a user of NICE org 
  I can use the guidance lists

  Background:
    Given I have a screen that is 1366 by 768 pixels

  Scenario Outline: Navigate to each guidance list tab
    Given I open the url "/guidance/<url>"
    Then I expect that the attribute "class" from element "#<id>" is "active listTab"
    Examples:
        | url | id |
        | published | linkPublished |
        | inconsultation | linkConsultations |
        | indevelopment | linkUpdatedGuidance |
        | proposed | linkProposed |
