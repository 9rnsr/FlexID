# 体内動態モデル間のコンパートメント対応関係

```
Pb-212 --> Bi-212 -+-> Po-212 -+
                    \           \
                     +-> Tl-208 -+-> (Pb-208)
```

モデル間で同一と識別できるコンパートメント。

|Pb-212         |Bi-212    |Po-212    |Tl-208    |
|---------------|----------|----------|----------|
|               |          |Plasma1   |          |
|Plasma         |Plasma    |Plasma2   |Plasma    |
|               |          |Plasma3   |          |
|RBC            |RBC       |RBC       |RBC       |
|C-bone-S       |C-bone-S  |C-bone-S  |C-bone-S  |
|T-bone-S       |T-bone-S  |T-bone-S  |T-bone-S  |
|Exch-C-bone-V  |          |          |          |
|Exch-T-bone-V  |          |          |          |
|Noch-C-bone-V  |          |          |          |
|Noch-T-bone-V  |          |          |          |
|Liver1         |Liver1    |Liver1    |Liver     |
|Liver2         |Liver2    |Liver2    |          |
|Kidneys1       |Kidneys1  |Kidneys1  |          |
|Kidneys2       |Kidneys2  |Kidneys2  |Kidneys   |
|               |RedMarrow |RedMarrow |RedMarrow |
|               |Testes    |Testes    |Testes    |
|               |Ovaries   |Ovaries   |Ovaries   |
|               |Skin      |Skin      |Skin      |
|               |Spleen    |Spleen    |Spleen    |


軟組織は異なる元素のモデル間では同一と識別できないものとして扱う。

|Pb-212         |Bi-212    |Po-212    |Tl-208    |
|---------------|----------|----------|----------|
|ST0            |ST0       |Other     |Other     |
|ST1            |ST1       |          |          |
|ST2            |ST2       |          |          |
