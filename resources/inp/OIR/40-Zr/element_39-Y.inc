# ICRP Publ.134 p.275 Para.577
# > (577) Chain members addressed in the derivation of dose coefficients for internally
# > deposited zirconium isotopes include isotopes of yttrium, strontium, and niobium.
# > The characteristic systemic models for yttrium, zirconium, and niobium all have the
# > same model structure. ...
# > (577) 体内に沈着したジルコニウム同位体の線量係数の導出において扱われる連鎖メンバーには、
# > イットリウム、ストロンチウム、およびニオブの同位体が含まれます。イットリウム、ジルコニウム、
# > およびニオブの特性全身モデルはすべて同じモデル構造を有します。 ...

[{Y}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver0                Liver
  acc   Liver1                Liver
  acc   Kidneys               Kidneys
  acc   ST0                   Other
  acc   ST1                   Other


[{Y}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.252 Table 11.3
  Blood1                  Blood2                    0.498
  Blood1                  Liver0                    1.66
  Blood1                  Kidneys                   0.166
  Blood1                  ST0                       3.652
  Blood1                  ST1                       1.328
  Blood1                  UB-con                    2.49
  Blood1                  SI-conRe                  0.166
  Blood1                  T-bone-S                  3.32
  Blood1                  C-bone-S                  3.32
  Blood2                  Blood1                    0.462
  Liver0                  SI-conRe                  0.0231
  Liver0                  Blood1                    0.0924
  Liver0                  Liver1                    0.116
  Liver1                  Blood1                    0.0019
  Kidneys                 Blood1                    0.0019
  ST0                     Blood1                    0.231
  ST1                     Blood1                    0.0019
  T-bone-S                Blood1                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood1                    0.000493
  C-bone-S                Blood1                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood1                    0.0000821

$if progeny

# ICRP Publ.134 p.275 Para.577
# > (577) ... An yttrium or niobium atom produced in a given compartment
# > by radioactive decay after intake of a zirconium parent is assumed to behave as
# > if it had entered that compartment as a parent radionuclide. ...
# > (577) ... ジルコニウム親核種の摂取後に
# > 放射性崩壊によって特定のコンパートメント内で生成されたイットリウムまたはニオブ原子は、
# > あたかも親核種としてそのコンパートメントに入ったかのように振舞うと仮定されます。 ...

  {Zr}/Blood1             Blood1                    ---
  {Zr}/Blood2             Blood2                    ---

  {Zr}/C-bone-S           C-bone-S                  ---
  {Zr}/T-bone-S           T-bone-S                  ---
  {Zr}/C-bone-V           C-bone-V                  ---
  {Zr}/T-bone-V           T-bone-V                  ---

  {Zr}/Liver0             Liver0                    ---
  {Zr}/Liver1             Liver1                    ---
  {Zr}/Kidneys            Kidneys                   ---

  {Zr}/ST0                ST0                       ---
  {Zr}/ST1                ST1                       ---

$endif
