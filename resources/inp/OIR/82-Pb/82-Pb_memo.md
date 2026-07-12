# 体内動態モデル間のコンパートメント対応関係

> ICRP Publ.137 p.227 Para.475
> (475) Several lead isotopes addressed in this publication have radioactive progeny
> that contribute, to some extent, to dose coefficients for the internally deposited lead
> parent. The radioactive progeny addressed in calculation of dose coefficients for
> radioisotopes of lead are isotopes of gold, mercury, thallium, lead, bismuth, and
> polonium.

Publ.137 Para.475 に従い、鉛の子孫として生成されるプラチナとオスミウムは考慮しない。

元素のモデル間で同一と識別できるコンパートメント。

|84-Po     |83-Bi     |82-Pb         |81-Tl     |80-Hg     |79-Au     |
|----------|----------|--------------|----------|----------|----------|
|          |Plasma    |Plasma        |Plasma    |Plasma1   |Blood1    |
|          |          |              |          |Plasma2   |          |
|Plasma1   |          |              |          |          |          |
|Plasma2   |          |              |          |          |          |
|Plasma3   |          |              |          |          |          |
|RBC       |RBC       |RBC           |RBC       |RBC       |          |
|          |          |              |          |          |Blood2    |
|C-bone-S  |C-bone-S  |C-bone-S      |C-bone-S  |C-bone-S  |C-bone-S  |
|T-bone-S  |T-bone-S  |T-bone-S      |T-bone-S  |T-bone-S  |T-bone-S  |
|          |          |Exch-C-bone-V |          |          |          |
|          |          |Exch-T-bone-V |          |          |          |
|          |          |Noch-C-bone-V |          |          |          |
|          |          |Noch-T-bone-V |          |          |          |
|Liver1    |Liver1    |Liver1        |Liver     |Liver     |Liver     |
|Liver2    |Liver2    |Liver2        |          |          |          |
|Kidneys1  |Kidneys1  |Kidneys1      |          |Kidneys   |Kidneys   |
|Kidneys2  |Kidneys2  |Kidneys2      |Kidneys   |          |          |
|RedMarrow |RedMarrow |RedMarrow  *1 |RedMarrow |RedMarrow |RedMarrow |
|Testes    |Testes    |Testes     *1 |Testes    |Testes    |Testes    |
|Ovaries   |Ovaries   |Ovaries    *1 |Ovaries   |Ovaries   |Ovaries   |
|Skin      |Skin      |Skin       *1 |Skin      |Skin      |Skin      |
|Spleen    |Spleen    |Spleen     *1 |Spleen    |Spleen    |Spleen    |

*1 子孫核種の場合のみ追加される

線源領域Otherは、異なる元素のモデル間では同一と識別できないものとして扱う。

|84-Po     |83-Bi     |82-Pb         |81-Tl     |80-Hg     |79-Au     |
|----------|----------|--------------|----------|----------|----------|
|Other     |ST0       |ST0           |Other     |Other1    |Other1    |
|          |ST1       |ST1           |          |Other2    |Other2    |
|          |ST2       |ST2           |          |          |          |
