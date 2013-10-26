KeywordParser
=============

NLP keyword parser using Stanford CoreNLP postagger in C#

Given some rest api that produces xml listings, i capture the description and parse 
keywords out of it with a naive parser over the stanford postagger as a preprocessor.

e.g.
Sentence: [Set/NNP, on/IN, 8,300/CD, square/JJ, feet/NNS, of/IN, magnificently/RB, landscaped/VBN, property/NN, ;/:, this/DT, 2/CD, level/NN, ,/,, 4/CD, bedroom/NN, home/
NN, is/VBZ, in/IN, the/DT, prime/JJ, of/IN, Marlborough/NNP, Heights/NNP, ./.]
Sentence: [Having/VBG, numerous/JJ, updates/NNS, over/IN, the/DT, years/NNS, ;/:, you/PRP, will/MD, find/VB, refinished/JJ, floors/NNS, ,/,, upgraded/VBN, kitchen/NN, and
/CC, baths/NNS, ,/,, and/CC, new/JJ, exterior/NN, paint/NN, ./.]
Sentence: [The/DT, very/RB, private/JJ, backyard/NN, boasts/VBZ, palm/NN, trees/NNS, ,/,, fantastic/JJ, entertaining/JJ, decks/NNS, and/CC, a/DT, top/NN, of/IN, the/DT, l
ine/NN, Jacuzzi/NN, hot/JJ, tub/NN, ./.]
Sentence: [This/DT, Upper/NNP, Lonsdale/NNP, home/NN, is/VBZ, centrally/RB, located/VBN, close/RB, to/TO, three/CD, schools/NNS, and/CC, offers/VBZ, much/JJ, potential/NN
, for/IN, future/JJ, development/NN, ./.]

<ul>
  <li>keywords: 2 level</li>
  <li>keywords: 4 bedroom home</li>
  <li>keywords: prime of marlborough heights</li>
  <li>keywords: set on 8,300 square feet</li>
  <li>keywords: magnificently landscaped property</li>
  <li>keywords: numerous updates</li>
  <li>keywords: refinished floors</li>
  <li>keywords: new exterior paint</li>
  <li>keywords: fantastic entertaining decks</li>
  <li>keywords: hot tub</li>
  <li>keywords: very private backyard</li>
  <li>keywords: much potential</li>
  <li>keywords: future development</li>
  <li>keywords: upper lonsdale home</li>
  <li>keywords: centrally located close to three schools</li>
</ul>

It would help to train it on relevant text first, tweak the tags and then run it again, but for an initial run this is not bad.


