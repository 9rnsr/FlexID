# 体内動態モデル間のコンパートメント対応関係

```
Th-232 --> Ra-228 --> Ac-228 --> Th-228 -+
                                          \
     +-------------------------------------+
      \
       +-> Ra-224 --> Rn-220 --> Po-216 -+
                                          \
     +-------------------------------------+
      \
       +-> Pb-212 --> Bi-212 -+-> Po-212 -+
                                \           \
                                 +-> Tl-208 -+-> (Pb-208)
```

全身モデル間で同一と扱うコンパートメント。

|Th-232   |Ra-228        |Ac-228   |Th-228   |Ra-224        |Rn-220 |Po-216   |Pb-212        |Bi-212   |Po-212   |Tl-208   |
|---------|--------------|---------|---------|--------------|-------|---------|--------------|---------|---------|---------|
|Blood    |Blood         |Blood    |Blood    |Blood         |Blood  |         |              |         |         |         |
|         |              |         |         |              |       |Plasma1  |              |         |Plasma1  |         |
|         |              |         |         |              |       |Plasma2  |              |         |Plasma2  |         |
|         |              |         |         |              |       |Plasma3  |              |         |Plasma3  |         |
|         |              |         |         |              |       |         |Plasma        |Plasma   |         |Plasma   |
|         |              |         |         |              |       |RBC      |RBC           |RBC      |RBC      |RBC      |
|C-bone-S |C-bone-S      |C-bone-S |C-bone-S |C-bone-S      |       |C-bone-S |C-bone-S      |C-bone-S |C-bone-S |C-bone-S |
|T-bone-S |T-bone-S      |T-bone-S |T-bone-S |T-bone-S      |       |T-bone-S |T-bone-S      |T-bone-S |T-bone-S |T-bone-S |
|         |Exch-C-bone-V |         |         |Exch-C-bone-V |       |         |Exch-C-bone-V |         |         |         |
|         |Exch-T-bone-V |         |         |Exch-T-bone-V |       |         |Exch-T-bone-V |         |         |         |
|C-bone-V |Noch-C-bone-V |C-bone-V |C-bone-V |Noch-C-bone-V |       |         |Noch-C-bone-V |         |         |         |
|T-bone-V |Noch-T-bone-V |T-bone-V |T-bone-V |Noch-T-bone-V |       |         |Noch-T-bone-V |         |         |         |
|C-marrow |C-marrow      |C-marrow |C-marrow |C-marrow      |       |C-marrow |C-marrow      |C-marrow |C-marrow |C-marrow |
|T-marrow |T-marrow      |T-marrow |T-marrow |T-marrow      |       |T-marrow |T-marrow      |T-marrow |T-marrow |T-marrow |
|Liver1   |Liver1        |Liver1   |Liver1   |Liver1        |       |Liver1   |Liver1        |Liver1   |Liver1   |Liver    |
|Liver2   |Liver2        |Liver2   |Liver2   |Liver2        |       |Liver2   |Liver2        |Liver2   |Liver2   |↗        |
|Kidneys1 |Kidneys1      |Kidneys1 |Kidneys1 |Kidneys1      |       |Kidneys1 |Kidneys1      |Kidneys1 |Kidneys1 |Kidneys  |
|Kidneys2 |Kidneys2      |Kidneys2 |Kidneys2 |Kidneys2      |       |Kidneys2 |Kidneys2      |Kidneys2 |Kidneys2 |↗        |
|Testes   |Testes        |Testes   |Testes   |Testes        |       |Testes   |Testes        |Testes   |Testes   |Testes   |
|Ovaries  |Ovaries       |Ovaries  |Ovaries  |Ovaries       |       |Ovaries  |Ovaries       |Ovaries  |Ovaries  |Ovaries  |
|         |Skin          |Skin     |Skin     |Skin          |       |Skin     |Skin          |Skin     |Skin     |Skin     |
|         |Spleen        |Spleen   |Spleen   |Spleen        |       |Spleen   |Spleen        |Spleen   |Spleen   |Spleen   |


軟組織は異なる元素の全身モデル間では同一と識別できないものとして扱う。

|Th-232   |Ra-228        |Ac-228   |Th-228   |Ra-224        |Rn-220 |Po-216   |Pb-212        |Bi-212   |Po-212   |Tl-208   |
|---------|--------------|---------|---------|--------------|-------|---------|--------------|---------|---------|---------|
|ST0      |ST0           |ST0      |ST0      |ST0           |       |Other    |ST0           |ST0      |Other    |Other    |
|ST1      |ST1           |ST1      |ST1      |ST1           |       |         |ST1           |ST1      |         |         |
|ST2      |ST2           |ST2      |ST2      |ST2           |       |         |ST2           |ST2      |         |         |
