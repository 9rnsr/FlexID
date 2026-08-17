# ICRP Publ.134 p.254 Para.529
# > (529) Chain members addressed in the derivation of dose coefficients for internally
# > deposited yttrium isotopes include isotopes of yttrium, strontium, zirconium, and
# > niobium. An yttrium isotope produced in the body after uptake of an yttrium parent
# > is assumed to have the same systemic biokinetics as the parent. Isotopes of zirconium
# > and niobium produced in systemic compartments after intake of an yttrium parent
# > are assigned the characteristic systemic models for zirconium and niobium, respectively,
# > described elsewhere in this publication. The characteristic systemic models for
# > yttrium, zirconium, and niobium all have the same model structure. ...
# > ICRP Publ.134 p.254 Para.529
# > (529) 体内に取り込まれたイットリウム同位体の線量係数の導出において対象とされる崩壊系列の
# > 核種には、イットリウム、ストロンチウム、ジルコニウム、およびニオブの同位体が含まれます。
# > イットリウム親核種の摂取後に体内で生成されるイットリウム同位体は、その親核種と同じ全身の
# > 生体内動態を示すものと仮定されます。イットリウム親核種の摂取後に全身コンパートメント内で
# > 生成されるジルコニウムおよびニオブの同位体には、本報告書の他の箇所で記述されている、
# > それぞれジルコニウムおよびニオブに固有の全身モデルが適用されます。イットリウム、
# > ジルコニウム、およびニオブに固有の全身モデルは、いずれも同じモデル構造を有しています。 ...

[{Zr}:compartment]
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


[{Zr}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.274 Table 12.3
  Blood1                  Blood2                    2.0
  Blood1                  Liver0                    0.075
  Blood1                  Kidneys                   0.0125
  Blood1                  ST0                       2.0
  Blood1                  ST1                       0.0375
  Blood1                  UB-con                    0.1
  Blood1                  SI-conRe                  0.025
  Blood1                  T-bone-S                  0.375
  Blood1                  C-bone-S                  0.375
  Blood2                  Blood1                    0.462
  Liver0                  SI-conRe                  0.116
  Liver0                  Blood1                    0.116
  Liver0                  Liver1                    0.462
  Liver1                  Blood1                    0.01
  Kidneys                 Blood1                    0.01
  ST0                     Blood1                    0.462
  ST1                     Blood1                    0.02
  T-bone-S                Blood1                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood1                    0.000493
  C-bone-S                Blood1                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood1                    0.0000821

$if progeny

# ICRP Publ.134 p.254 Para.529
# > (529) ... A zirconium or
# > niobium atom produced in a compartment by radioactive decay is assumed to
# > behave as if it had entered that compartment as a parent radionuclide. This includes
# > subcompartments of ‘other soft tissues’.
# > (529) ... 放射性崩壊によってコンパートメント内で生成されたジルコニウムまたはニオブの原子は、
# > 親放射性核種としてそのコンパートメントに流入した場合と同様に挙動すると仮定されます。
# > これには、「その他の軟部組織」のサブコンパートメントも含まれます。

  {Y,Zr,Nb}/Blood1        Blood1                    ---
  {Y,Zr,Nb}/Blood2        Blood2                    ---

  {Y,Zr,Nb}/C-bone-S      C-bone-S                  ---
  {Y,Zr,Nb}/T-bone-S      T-bone-S                  ---
  {Y,Zr,Nb}/C-bone-V      C-bone-V                  ---
  {Y,Zr,Nb}/T-bone-V      T-bone-V                  ---

  {Y,Zr,Nb}/Liver0        Liver0                    ---
  {Y,Zr,Nb}/Liver1        Liver1                    ---
  {Y,Zr,Nb}/Kidneys       Kidneys                   ---

  {Y,Zr,Nb}/ST0           ST0                       ---
  {Y,Zr,Nb}/ST1           ST1                       ---

$endif
