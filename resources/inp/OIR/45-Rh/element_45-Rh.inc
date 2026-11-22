[{Rh}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissue
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Rh}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.216 Table 23.3
  Blood1                  SI-conRe                  3.0
  Blood1                  UB-con                   17
  Blood1                  Liver1                   12
  Blood1                  Kidneys1                  7.76
  Blood1                  Kidneys2                  0.24
  Blood1                  Blood2                   27
  Blood1                  ST0                      15
  Blood1                  ST1                       5.0
  Blood1                  ST2                       5.0
  Blood1                  C-bone-S                  2.0
  Blood1                  T-bone-S                  6.0
  Blood2                  Blood1                    0.6931
  Liver1                  Blood1                    0.09704
  Liver1                  SI-conRe                  0.03466
  Liver1                  Liver2                    0.006931
  Liver2                  Blood1                    0.003798
  Kidneys1                UB-con                    0.1386
  Kidneys2                Blood1                    0.003798
  ST0                     Blood1                    0.09902
  ST1                     Blood1                    0.0231
  ST2                     Blood1                    0.0009495
  C-bone-S                Blood1                    0.07922
  T-bone-S                Blood1                    0.07922
  C-bone-S                C-bone-V                  0.0198
  T-bone-S                T-bone-V                  0.0198
  C-bone-V                Blood1                    0.0000821
  T-bone-V                Blood1                    0.000493

$if progeny

# ICRP Publ.151 p.215 Para.421
# > (421) Progeny of rhodium addressed in this publication are isotopes of rhodium,
# > ruthenium, and technetium. The common model for rhodium and ruthenium as
# > parents applied in the OIR series is applied to isotopes of these two elements as
# > progeny of rhodium. The model for technetium as a progeny of ruthenium applied in
# > OIR Part 3 (ICRP, 2017) is applied to technetium as a progeny of rhodium.
# > (421) 本出版物で扱うロジウムの娘核種は、ロジウム、ルテニウム、およびテクネチウムの
# > 同位体です。OIRシリーズにおいて、親核種としてのロジウムおよびルテニウムに対して
# > 用いられている共通のモデルが、ロジウムの娘核種であるこれら2元素の同位体にも適用されます。
# > また、OIR第3部（ICRP, 2017）において、ルテニウムの娘核種であるテクネチウムに対して
# > 用いられているモデルが、ロジウムの娘核種であるテクネチウムにも適用されます。

  {Rh}/Blood1             Blood1                    ---
  {Rh}/Blood2             Blood2                    ---

  {Rh}/C-bone-S           C-bone-S                  ---
  {Rh}/T-bone-S           T-bone-S                  ---
  {Rh}/C-bone-V           C-bone-V                  ---
  {Rh}/T-bone-V           T-bone-V                  ---

  {Rh}/Liver1             Liver1                    ---
  {Rh}/Liver2             Liver2                    ---
  {Rh}/Kidneys1           Kidneys1                  ---
  {Rh}/Kidneys2           Kidneys2                  ---

  {Rh}/ST0                ST0                       ---
  {Rh}/ST1                ST1                       ---
  {Rh}/ST2                ST2                       ---

$endif
