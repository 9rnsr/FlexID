[{*}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   ET1-F                 ET1-sur
  acc   ET2-F                 ET2-sur
  acc   ETseq-F               ET2-seq
  acc   LNET-F                LN-ET
  acc   BB-F                  Bronchi
  acc   BBseq-F               Bronchi-q
  acc   bb-F                  Brchiole
  acc   bbseq-F               Brchiole-q
  acc   ALV-F                 ALV
  acc   INT-F                 ALV
  acc   LNTH-F                LN-Th
  acc   ET1-S                 ET1-sur
  acc   ET2-S                 ET2-sur
  acc   ETseq-S               ET2-seq
  acc   LNET-S                LN-ET
  acc   BB-S                  Bronchi
  acc   BBseq-S               Bronchi-q
  acc   bb-S                  Brchiole
  acc   bbseq-S               Brchiole-q
  acc   ALV-S                 ALV
  acc   INT-S                 ALV
  acc   LNTH-S                LN-Th
  acc   ET2-B                 ET2-bnd
  acc   ETseq-B               ET2-seq
  acc   LNET-B                LN-ET
  acc   BB-B                  Bronchi
  acc   BBseq-B               Bronchi-b
  acc   bb-B                  Brchiole
  acc   bbseq-B               Brchiole-b
  acc   ALV-B                 ALV
  acc   INT-B                 ALV
  acc   LNTH-B                LN-Th
  exc   Environment           ---

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

# ICRP Publ.130 p.65 Fig.3.4
# ICRP Publ.130 p.77 Para.143 & p.75 Fig.3.6
  ALV-F                   bb-F                      0.002
  ALV-F                   INT-F                     0.001
  INT-F                   LNTH-F                    0.00003
  bb-F                    BB-F                      0.2
  bbseq-F                 LNTH-F                    0.001
  BB-F                    ET2-F                    10
  BBseq-F                 LNTH-F                    0.001
  ET2-F                   Oesophagus-s            100
  ETseq-F                 LNET-F                    0.001
  ET1-F                   Environment               0.6
  ET1-F                   ET2-F                     1.5

  ALV-S                   bb-S                      0.002
  ALV-S                   INT-S                     0.001
  INT-S                   LNTH-S                    0.00003
  bb-S                    BB-S                      0.2
  bbseq-S                 LNTH-S                    0.001
  BB-S                    ET2-S                    10
  BBseq-S                 LNTH-S                    0.001
  ET2-S                   Oesophagus-s            100
  ETseq-S                 LNET-S                    0.001
  ET1-S                   Environment               0.6
  ET1-S                   ET2-S                     1.5

# ICRP Publ.130 p.67 Fig.3.5(a)
# ICRP Publ.130 p.68 Para.107
  ALV-F                   ALV-B                 $(      fb  * sr )
  INT-F                   INT-B                 $(      fb  * sr )
  bb-F                    bb-B                  $(      fb  * sr )
  bbseq-F                 bbseq-B               $(      fb  * sr )
  BB-F                    BB-B                  $(      fb  * sr )
  BBseq-F                 BBseq-B               $(      fb  * sr )
  ET2-F                   ET2-B                 $(      fb  * sr )
  ETseq-F                 ETseq-B               $(      fb  * sr )
  LNET-F                  LNET-B                $(      fb  * sr )
  LNTH-F                  LNTH-B                $(      fb  * sr )

  ALV-S                   ALV-B                 $(      fb  * ss )
  INT-S                   INT-B                 $(      fb  * ss )
  bb-S                    bb-B                  $(      fb  * ss )
  bbseq-S                 bbseq-B               $(      fb  * ss )
  BB-S                    BB-B                  $(      fb  * ss )
  BBseq-S                 BBseq-B               $(      fb  * ss )
  ET2-S                   ET2-B                 $(      fb  * ss )
  ETseq-S                 ETseq-B               $(      fb  * ss )
  LNET-S                  LNET-B                $(      fb  * ss )
  LNTH-S                  LNTH-B                $(      fb  * ss )

  ALV-F                   $Blood                $( (1 - fb) * sr )
  INT-F                   $Blood                $( (1 - fb) * sr )
  bb-F                    $Blood                $( (1 - fb) * sr )
  bbseq-F                 $Blood                $( (1 - fb) * sr )
  BB-F                    $Blood                $( (1 - fb) * sr )
  BBseq-F                 $Blood                $( (1 - fb) * sr )
  ET2-F                   $Blood                $( (1 - fb) * sr )
  ETseq-F                 $Blood                $( (1 - fb) * sr )
  LNET-F                  $Blood                $( (1 - fb) * sr )
  LNTH-F                  $Blood                $( (1 - fb) * sr )

  ALV-S                   $Blood                $( (1 - fb) * ss )
  INT-S                   $Blood                $( (1 - fb) * ss )
  bb-S                    $Blood                $( (1 - fb) * ss )
  bbseq-S                 $Blood                $( (1 - fb) * ss )
  BB-S                    $Blood                $( (1 - fb) * ss )
  BBseq-S                 $Blood                $( (1 - fb) * ss )
  ET2-S                   $Blood                $( (1 - fb) * ss )
  ETseq-S                 $Blood                $( (1 - fb) * ss )
  LNET-S                  $Blood                $( (1 - fb) * ss )
  LNTH-S                  $Blood                $( (1 - fb) * ss )

  ALV-B                   $Blood                $sb
  INT-B                   $Blood                $sb
  bb-B                    $Blood                $sb
  bbseq-B                 $Blood                $sb
  BB-B                    $Blood                $sb
  BBseq-B                 $Blood                $sb
  ET2-B                   $Blood                $sb
  ETseq-B                 $Blood                $sb
  LNET-B                  $Blood                $sb
  LNTH-B                  $Blood                $sb

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
  SI-con                  $Blood                $(fA    * 6 / (1 - fA   ))
  SI-conRe                $Blood                $(fA_Re * 6 / (1 - fA_Re))

# ICRP Publ.130 p.85 Para.172
  UB-con                  Urine                    12


[{*}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# decay for HRTM
  {*}/ET1-F               ET1-F                     ---
  {*}/ET2-F               ET2-F                     ---
  {*}/ETseq-F             ETseq-F                   ---
  {*}/LNET-F              LNET-F                    ---
  {*}/BB-F                BB-F                      ---
  {*}/BBseq-F             BBseq-F                   ---
  {*}/bb-F                bb-F                      ---
  {*}/bbseq-F             bbseq-F                   ---
  {*}/ALV-F               ALV-F                     ---
  {*}/INT-F               INT-F                     ---
  {*}/LNTH-F              LNTH-F                    ---
  {*}/ET1-S               ET1-S                     ---
  {*}/ET2-S               ET2-S                     ---
  {*}/ETseq-S             ETseq-S                   ---
  {*}/LNET-S              LNET-S                    ---
  {*}/BB-S                BB-S                      ---
  {*}/BBseq-S             BBseq-S                   ---
  {*}/bb-S                bb-S                      ---
  {*}/bbseq-S             bbseq-S                   ---
  {*}/ALV-S               ALV-S                     ---
  {*}/INT-S               INT-S                     ---
  {*}/LNTH-S              LNTH-S                    ---
  {*}/ET2-B               ET2-B                     ---
  {*}/ETseq-B             ETseq-B                   ---
  {*}/LNET-B              LNET-B                    ---
  {*}/BB-B                BB-B                      ---
  {*}/BBseq-B             BBseq-B                   ---
  {*}/bb-B                bb-B                      ---
  {*}/bbseq-B             bbseq-B                   ---
  {*}/ALV-B               ALV-B                     ---
  {*}/INT-B               INT-B                     ---
  {*}/LNTH-B              LNTH-B                    ---
  {*}/Environment         Environment               ---

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
