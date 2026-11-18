[include:common_6-C_parameter.inc]

[parameter]
  # ICRP Publ.134 p.53 Table 3.3 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0                   # footnote†
  $sb = 0                   # footnote†

  ${C}/fA    = 0.2
  ${C}/fA_Re = fA_MaxValueOfIng     # footnote††


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_6-C_Generic.inc]
