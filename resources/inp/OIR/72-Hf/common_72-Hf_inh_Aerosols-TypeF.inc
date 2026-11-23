[include:common_72-Hf_parameter.inc]

[parameter]
  # ICRP Publ.151 p.268 Table 29.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Hf}/fA    = 0.002
  ${Hf}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_73-Ta.inc]
[include:element_72-Hf.inc]
[include:element_71-Lu.inc]
