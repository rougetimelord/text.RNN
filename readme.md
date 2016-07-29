# text.RNN by rouge [![Build Status](https://travis-ci.org/rougetimelord/text.RNN.svg?branch=master)](https://travis-ci.org/rougetimelord/text.RNN)
A simple little (psuedo)RNN that chooses the next word dependent on the current word based on what it's seen.
### A brief explanation of the three parts of this project:
###### text.Learner:
- Learns text by assigning point values based on precentage of text

###### text.Speaker:
- Outputs text trying to mimic input

###### text.Scorer:
- Lets the user update the scores of words to achieve better readiblity
