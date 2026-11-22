[include:common_34-Se_parameter.inc]

[parameter]
  # ICRP Publ.151 p.186 Table 20.2 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 0.0001
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Se}/fA    = 0.008
  ${Se}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_36-Kr.inc]
[include:element_35-Br.inc]
[include:element_34-Se.inc]
[include:element_33-As.inc]
