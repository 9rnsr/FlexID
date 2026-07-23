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

[{^Rn,Fr,At}:transfer]
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
