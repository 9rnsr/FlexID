[include:common_80-Hg_parameter.inc]

[parameter]
  # ICRP Publ.151 p.326 Table 36.2 / Mercury vapor

  $fr = 0.94
  $sr = 1000
  $ss = 0.14
  $fb = 0.24                # footnote†
  $sb = 2.1                 # footnote†

$if parent
  ${Hg}/fA    = 0.094
  ${Hg}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.151 p.326 Table 36.2
# ICRP Publ.130 p.64 Para.98 & p.65 Fig.3.4 footnote
# > Rates shown alongside arrows are reference values in units of d⁻¹. It is assumed that 0.2% of
# > material deposited in the posterior nasal passage, pharynx, and larynx (ET2), bronchi (BB),
# > and bronchioles (bb) is retained in the airway wall (ETseq, BBseq, and bbseq, respectively).
# > 矢印と共に示された速度は、d⁻¹ を単位とする参照値である。後鼻腔、咽頭、喉頭(ET2)、気管支(BB)、
# > 細気管支(bb)に沈着した物質の0.2%が気道壁に滞留すると仮定する(それぞれETseq、BBseq、bbseq)。
  ET1-F                 $(      fr  *                  0% )
  ET2-F                 $(      fr  * (100% - 0.2%) *  2% )
  ETseq-F               $(      fr  *         0.2%  *  2% )
  BB-F                  $(      fr  * (100% - 0.2%) *  1% )
  BBseq-F               $(      fr  *         0.2%  *  1% )
  bb-F                  $(      fr  * (100% - 0.2%) *  2% )
  bbseq-F               $(      fr  *         0.2%  *  2% )
  ALV-F                 $(      fr  *                 75% )

  ET1-S                 $( (1 - fr) *                  0% )
  ET2-S                 $( (1 - fr) * (100% - 0.2%) *  2% )
  ETseq-S               $( (1 - fr) *         0.2%  *  2% )
  BB-S                  $( (1 - fr) * (100% - 0.2%) *  1% )
  BBseq-S               $( (1 - fr) *         0.2%  *  1% )
  bb-S                  $( (1 - fr) * (100% - 0.2%) *  2% )
  bbseq-S               $( (1 - fr) *         0.2%  *  2% )
  ALV-S                 $( (1 - fr) *                 75% )

  Environment           $(                     100% - 80% )


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


[{Hg}:transfer]
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

# ICRP Publ.151 p.336 Para.698
# > (698) The fraction of inhaled mercury vapour that is absorbed rapidly into blood,
# > fr(1-fb), enters the systemic circulation as mercury vapour through ‘Plasma 0’, while
# > the slowly absorbed fraction, (1-fr)(1-fb), and the bound fraction, fb, enter the
# > systemic circulation through ‘Plasma 1’ as mercury vapour is changed to divalent
# > mercury in the lung tissues.
# > (698) 吸入された水銀蒸気のうち、急速に血中に吸収される画分fr(1-fb)は、水銀蒸気として
# > Plasma 0コンパートメントを介して全身循環に入るが、ゆっくりと吸収される画分(1-fr)(1-fb)と
# > 結合画分fbは、水銀蒸気が肺組織で2価の水銀に変化するため、Plasma 1コンパートメントを介して
# > 全身循環に入る。
  ALV-F                   Plasma0               $( (1 - fb) * sr )
  INT-F                   Plasma0               $( (1 - fb) * sr )
  bb-F                    Plasma0               $( (1 - fb) * sr )
  bbseq-F                 Plasma0               $( (1 - fb) * sr )
  BB-F                    Plasma0               $( (1 - fb) * sr )
  BBseq-F                 Plasma0               $( (1 - fb) * sr )
  ET2-F                   Plasma0               $( (1 - fb) * sr )
  ETseq-F                 Plasma0               $( (1 - fb) * sr )
  LNET-F                  Plasma0               $( (1 - fb) * sr )
  LNTH-F                  Plasma0               $( (1 - fb) * sr )

  ALV-S                   Plasma1               $( (1 - fb) * ss )
  INT-S                   Plasma1               $( (1 - fb) * ss )
  bb-S                    Plasma1               $( (1 - fb) * ss )
  bbseq-S                 Plasma1               $( (1 - fb) * ss )
  BB-S                    Plasma1               $( (1 - fb) * ss )
  BBseq-S                 Plasma1               $( (1 - fb) * ss )
  ET2-S                   Plasma1               $( (1 - fb) * ss )
  ETseq-S                 Plasma1               $( (1 - fb) * ss )
  LNET-S                  Plasma1               $( (1 - fb) * ss )
  LNTH-S                  Plasma1               $( (1 - fb) * ss )

  ALV-B                   Plasma1               $sb
  INT-B                   Plasma1               $sb
  bb-B                    Plasma1               $sb
  bbseq-B                 Plasma1               $sb
  BB-B                    Plasma1               $sb
  BBseq-B                 Plasma1               $sb
  ET2-B                   Plasma1               $sb
  ETseq-B                 Plasma1               $sb
  LNET-B                  Plasma1               $sb
  LNTH-B                  Plasma1               $sb


[include:../share/common_HATM+UB.inc]

[include:element_80-Hg_GasVapor.inc]
[include:element_79-Au.inc]
[include:element_78-Pt.inc]
[include:element_76-Os.inc]
