# 体内動態モデル間のコンパートメント対応関係

```
Pb-214 --> Bi-214 -+-> Po-214 -+
                    \           \
                     +-> Tl-210 -+-> Pb-210 -+-> Bi-210 -+-> Po-210 -+
                                              \           \           \
                                               +-> Hg-206 -+-> Tl-206 -+-> (Pb-206)
```

モデル間で同一と識別できるコンパートメント。

|Pb-214         |Bi-214    |Po-214    |Tl-210    |Pb-210         |Bi-210    |Po-210    |Hg-206    |Tl-206    |
|---------------|----------|----------|----------|---------------|----------|----------|----------|----------|
|Plasma         |Plasma    |          |Plasma    |Plasma         |Plasma    |          |Plasma1   |Plasma    |
|               |          |          |          |               |          |          |Plasma2   |          |
|               |          |Plasma1   |          |               |          |Plasma1   |          |          |
|               |          |Plasma2   |          |               |          |Plasma2   |          |          |
|               |          |Plasma3   |          |               |          |Plasma3   |          |          |
|RBC            |RBC       |RBC       |RBC       |RBC            |RBC       |RBC       |RBC       |RBC       |
|C-bone-S       |C-bone-S  |C-bone-S  |C-bone-S  |C-bone-S       |C-bone-S  |C-bone-S  |C-bone-S  |C-bone-S  |
|T-bone-S       |T-bone-S  |T-bone-S  |T-bone-S  |T-bone-S       |T-bone-S  |T-bone-S  |T-bone-S  |T-bone-S  |
|Exch-C-bone-V  |          |          |          |Exch-C-bone-V  |          |          |          |          |
|Exch-T-bone-V  |          |          |          |Exch-T-bone-V  |          |          |          |          |
|Noch-C-bone-V  |          |          |          |Noch-C-bone-V  |          |          |          |          |
|Noch-T-bone-V  |          |          |          |Noch-T-bone-V  |          |          |          |          |
|Liver1         |Liver1    |Liver1    |Liver     |Liver1         |Liver1    |Liver1    |Liver     |Liver     |
|Liver2         |Liver2    |Liver2    |          |Liver2         |Liver2    |Liver2    |          |          |
|Kidneys1       |Kidneys1  |Kidneys1  |Kidneys   |Kidneys1       |Kidneys1  |Kidneys1  |Kidneys   |Kidneys   |
|Kidneys2       |Kidneys2  |Kidneys2  |          |Kidneys2       |Kidneys2  |Kidneys2  |          |          |
|               |RedMarrow |RedMarrow |RedMarrow |RedMarrow      |RedMarrow |RedMarrow |RedMarrow |RedMarrow |
|               |Testes    |Testes    |Testes    |Testes         |Testes    |Testes    |Testes    |Testes    |
|               |Ovaries   |Ovaries   |Ovaries   |Ovaries        |Ovaries   |Ovaries   |Ovaries   |Ovaries   |
|               |Skin      |Skin      |Skin      |Skin           |Skin      |Skin      |Skin      |Skin      |
|               |Spleen    |Spleen    |Spleen    |Spleen         |Spleen    |Spleen    |Spleen    |Spleen    |


軟組織は異なる元素のモデル間では同一と識別できないものとして扱う。

|Pb-214         |Bi-214    |Po-214    |Tl-210    |Pb-210         |Bi-210    |Po-210    |Hg-206    |Tl-206    |
|---------------|----------|----------|----------|---------------|----------|----------|----------|----------|
|ST0            |ST0       |Other     |Other     |ST0            |ST0       |Other     |Other1    |Other     |
|ST1            |ST1       |          |          |ST1            |ST1       |          |Other2    |          |
|ST2            |ST2       |          |          |ST2            |ST2       |          |          |          |
