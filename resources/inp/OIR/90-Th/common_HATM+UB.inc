[{*}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Oralcavity            O-cavity
  acc   Oesophagus-f          Oesophag-f
  acc   Oesophagus-s          Oesophag-s
  acc   St-con                St-cont
  acc   SI-con                SI-cont
  acc   SI-conRe              SI-cont
  acc   RC-con                RC-cont
  acc   LC-con                LC-cont
  acc   RS-con                RS-cont
  exc   Faeces                ---

  acc   UB-con                UB-cont
  exc   Urine                 ---


[{^He,Ne,Ar,Kr,Xe,Rn}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.130 p.76 Table 3.4 & footnote
  Oralcavity              Oesophagus-f           6480
  Oralcavity              Oesophagus-s            720
  Oesophagus-f            St-con                12343
  Oesophagus-s            St-con                 2160
  St-con                  SI-con                   20.57
  SI-con                  RC-con                    6
  SI-conRe                RC-con                    6
  RC-con                  LC-con                    2
  LC-con                  RS-con                    2
  RS-con                  Faeces                    2

# λ(SI->Blood) = fA * λ(SI->RC) / (1 - fA)
  SI-con                  $Blood        $(fA    * 6 / (1 - fA   ))
  SI-conRe                $Blood        $(fA_Re * 6 / (1 - fA_Re))

# ICRP Publ.130 p.85 Para.172
  UB-con                  Urine                    12


# ICRP Publ.130 p.45 Para.38
# > (38) Noble gases produced in compartments of the respiratory tract and in the
# > alimentary tract models by radioactive decay are assumed to escape from these
# > compartments directly to the environment at a rate of 100 d⁻¹, without transfer to
# > the blood compartment and without transfer between compartments of respiratory
# > tract and alimentary tract models. It is assumed that progeny of such noble gases
# > formed within the body follow the rules stated in Para. 37.
# > (38) 呼吸器系および消化管系モデルのコンパートメント内で放射性崩壊により生成される
# > 希ガスは、これらのコンパートメントから100 d⁻¹の速度で直接環境中へ放出され、
# > 血液コンパートメントへの移行や、呼吸器系および消化管系モデルのコンパートメント間での
# > 移行は起こらないものと仮定されます。体内において生成されるこれら希ガスの娘核種に
# > ついては、Para.37に述べた規則に従うものと仮定されます。

# ICRP Publ.130 Para.129
# > (129) For calculation purposes, the assumption that noble gases, including radon,
# > that are formed as progeny within the respiratory tract escape from the body at a rate
# > of 100 d⁻¹is applied in this series of reports.
# > (129) 計算のために、この一連の報告書では、呼吸器系内で子孫核種として生成される
# > ラドンを含む希ガスが100 d⁻¹の速度で体から排出されるという仮定を適用しています。

[{He,Ne,Ar,Kr,Xe,Rn}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

  Oralcavity              Exhaled                 100
  Oesophagus-f            Exhaled                 100
  Oesophagus-s            Exhaled                 100
  St-con                  Exhaled                 100
  SI-con                  Exhaled                 100
  SI-conRe                Exhaled                 100
  RC-con                  Exhaled                 100
  LC-con                  Exhaled                 100
  RS-con                  Exhaled                 100

# ICRP Publ.130 p.85 Para.172
  UB-con                  Exhaled                  12


[{*}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# decay for HATM
  {*}/Oralcavity          Oralcavity                ---
  {*}/Oesophagus-f        Oesophagus-f              ---
  {*}/Oesophagus-s        Oesophagus-s              ---
  {*}/St-con              St-con                    ---
  {*}/SI-con              SI-con                    ---
  {*}/SI-conRe            SI-conRe                  ---
  {*}/RC-con              RC-con                    ---
  {*}/LC-con              LC-con                    ---
  {*}/RS-con              RS-con                    ---
  {*}/Faeces              Faeces                    ---

# decay for UB
  {*}/UB-con              UB-con                    ---
  {*}/Urine               Urine                     ---
