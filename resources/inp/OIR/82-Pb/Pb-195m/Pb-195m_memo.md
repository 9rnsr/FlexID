# 体内動態モデル間のコンパートメント対応関係

```
Pb-195m --> Tl-195 -+-> Hg-195m -+-+-> Hg-195 -+-> Au-195 --> (Pt-195)
                     \            X           /
                      +----------+ +---------+
```

モデル間で同一と識別できるコンパートメント。

|Pb-195m        |Tl-195    |Hg-195m   |Hg-195    |Au-195    |
|---------------|----------|----------|----------|----------|
|Plasma         |Plasma    |Plasma1   |Plasma1   |Blood1    |
|               |          |Plasma2   |Plasma2   |          |
|RBC            |RBC       |RBC       |RBC       |          |
|               |          |          |          |Blood2    |
|C-bone-S       |C-bone-S  |C-bone-S  |C-bone-S  |C-bone-S  |
|T-bone-S       |T-bone-S  |T-bone-S  |T-bone-S  |T-bone-S  |
|Exch-C-bone-V  |          |          |          |          |
|Exch-T-bone-V  |          |          |          |          |
|Noch-C-bone-V  |          |          |          |          |
|Noch-T-bone-V  |          |          |          |          |
|Liver1         |Liver     |Liver     |Liver     |Liver     |
|Liver2         |          |          |          |          |
|Kidneys1       |          |Kidneys   |Kidneys   |Kidneys   |
|Kidneys2       |Kidneys   |          |          |          |
|               |RedMarrow |RedMarrow |RedMarrow |RedMarrow |
|               |Testes    |Testes    |Testes    |Testes    |
|               |Ovaries   |Ovaries   |Ovaries   |Ovaries   |
|               |Skin      |Skin      |Skin      |Skin      |
|               |Spleen    |Spleen    |Spleen    |Spleen    |


軟組織は異なる元素のモデル間では同一と識別できないものとして扱う。

|Pb-195m        |Tl-195    |Hg-195m   |Hg-195    |Au-195    |
|---------------|----------|----------|----------|----------|
|ST0            |Other     |Other1    |Other1    |Other1    |
|ST1            |          |Other2    |Other2    |Other2    |
|ST2            |          |          |          |          |
