Feature: Adis

Scenario: Create header
	Given the adis file:
	  | line                                      |
	  | DN001234000001110230000444405600077777889 |
	  | EN                                        |
	  | ZN                                        |
	Then definition with event number 1234 has the following columns:
	  | ddi   | length | resolution |
	  | 111   | 2      | 3          |
	  | 4444  | 5      | 6          |
	  | 77777 | 88     | 9          |