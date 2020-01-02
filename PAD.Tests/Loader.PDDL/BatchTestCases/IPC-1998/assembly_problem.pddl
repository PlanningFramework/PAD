﻿(define (problem assem-x-30)
   (:domain assembly)
   (:objects wire socket-47 mount-43 wire-44 unit-45 frob-46
             mount-53 gimcrack-48 socket-49 whatsis-50 wire-51 valve-52
             sprocket-60 foobar-54 unit-55 doodad-56 kludge-57 coil-58
             bracket-59 frob-63 thingumbob-61 contraption-62 connector-22
             plug-64 device-17 widget-18 gimcrack-19 hack-20
             hoozawhatsie-21 mount-29 fastener-23 whatsis-25 coil-27 hack-12
             fastener-30 plug-31 unit-32 tube-9 gimcrack-10 socket-11
             hoozawhatsie-7 contraption-13 widget-14 device-16 doodad-5
             sprocket-6 contraption frob-8 valve-3 wire-4 connector-1
             bracket-2 gimcrack unit sprocket tube fastener foobar connector
             frob thingumbob device widget hoozawhatsie coil plug mount
             socket valve bracket kludge hack - assembly
             file - resource)
   (:init (available mount-43)
          (available wire-44)
          (available unit-45)
          (available frob-46)
          (available gimcrack-48)
          (available socket-49)
          (available whatsis-50)
          (available wire-51)
          (available valve-52)
          (available foobar-54)
          (available unit-55)
          (available doodad-56)
          (available kludge-57)
          (available coil-58)
          (available bracket-59)
          (available thingumbob-61)
          (available contraption-62)
          (available plug-64)
          (available device-17)
          (available widget-18)
          (available gimcrack-19)
          (available hack-20)
          (available hoozawhatsie-21)
          (available fastener-23)
          (available whatsis-25)
          (available coil-27)
          (available fastener-30)
          (available plug-31)
          (available unit-32)
          (available tube-9)
          (available gimcrack-10)
          (available socket-11)
          (available contraption-13)
          (available widget-14)
          (available device-16)
          (available doodad-5)
          (available sprocket-6)
          (available frob-8)
          (available valve-3)
          (available wire-4)
          (available connector-1)
          (available bracket-2)
          (available gimcrack)
          (available tube)
          (available foobar)
          (available connector)
          (available device)
          (available widget)
          (available coil)
          (available mount)
          (available socket)
          (available valve)
          (available bracket)
          (available kludge)
          (available hack)
          (available file)
          (requires socket-47 file)
          (requires mount-53 file)
          (requires sprocket-60 file)
          (requires frob-63 file)
          (requires connector-22 file)
          (requires mount-29 file)
          (requires hack-12 file)
          (requires hoozawhatsie-7 file)
          (requires contraption file)
          (requires unit file)
          (requires frob file)
          (requires plug file)
          (part-of socket-47 wire)
          (part-of mount-53 wire)
          (part-of sprocket-60 wire)
          (part-of frob-63 wire)
          (part-of connector-22 wire)
          (part-of mount-29 wire)
          (part-of hack-12 wire)
          (part-of hoozawhatsie-7 wire)
          (part-of contraption wire)
          (part-of unit wire)
          (part-of frob wire)
          (part-of plug wire)
          (part-of mount-43 socket-47)
          (part-of wire-44 socket-47)
          (part-of unit-45 socket-47)
          (part-of frob-46 socket-47)
          (part-of gimcrack-48 mount-53)
          (part-of socket-49 mount-53)
          (part-of whatsis-50 mount-53)
          (part-of wire-51 mount-53)
          (part-of valve-52 mount-53)
          (part-of foobar-54 sprocket-60)
          (part-of unit-55 sprocket-60)
          (part-of doodad-56 sprocket-60)
          (part-of kludge-57 sprocket-60)
          (part-of coil-58 sprocket-60)
          (part-of bracket-59 sprocket-60)
          (part-of thingumbob-61 frob-63)
          (part-of contraption-62 frob-63)
          (part-of plug-64 connector-22)
          (part-of device-17 connector-22)
          (part-of widget-18 connector-22)
          (part-of gimcrack-19 connector-22)
          (part-of hack-20 connector-22)
          (part-of hoozawhatsie-21 connector-22)
          (part-of fastener-23 mount-29)
          (part-of gimcrack mount-29)
          (part-of whatsis-25 mount-29)
          (part-of doodad-5 mount-29)
          (part-of coil-27 mount-29)
          (part-of hoozawhatsie mount-29)
          (part-of fastener-30 hack-12)
          (part-of plug-31 hack-12)
          (part-of unit-32 hack-12)
          (part-of tube-9 hack-12)
          (part-of gimcrack-10 hack-12)
          (part-of socket-11 hack-12)
          (part-of contraption-13 hoozawhatsie-7)
          (part-of widget-14 hoozawhatsie-7)
          (part-of fastener hoozawhatsie-7)
          (part-of device-16 hoozawhatsie-7)
          (transient-part doodad-5 hoozawhatsie-7)
          (part-of sprocket-6 hoozawhatsie-7)
          (part-of frob-8 contraption)
          (part-of valve-3 contraption)
          (part-of wire-4 contraption)
          (part-of connector-1 contraption)
          (part-of bracket-2 contraption)
          (transient-part gimcrack contraption)
          (part-of sprocket unit)
          (transient-part fastener unit)
          (part-of connector sprocket)
          (part-of tube sprocket)
          (part-of foobar fastener)
          (transient-part connector fastener)
          (part-of thingumbob frob)
          (transient-part hoozawhatsie frob)
          (part-of device thingumbob)
          (transient-part widget thingumbob)
          (part-of widget hoozawhatsie)
          (part-of coil hoozawhatsie)
          (part-of mount plug)
          (part-of socket plug)
          (part-of valve plug)
          (part-of bracket plug)
          (part-of kludge plug)
          (part-of hack plug)
          (assemble-order socket-47 unit wire)
          (assemble-order socket-47 connector-22 wire)
          (assemble-order mount-53 frob wire)
          (assemble-order frob-63 sprocket-60 wire)
          (assemble-order connector-22 hoozawhatsie-7 wire)
          (assemble-order contraption unit wire)
          (assemble-order contraption mount-53 wire)
          (assemble-order frob socket-47 wire)
          (assemble-order plug contraption wire)
          (assemble-order gimcrack-48 wire-51 mount-53)
          (assemble-order socket-49 valve-52 mount-53)
          (assemble-order foobar-54 bracket-59 sprocket-60)
          (assemble-order foobar-54 coil-58 sprocket-60)
          (assemble-order coil-58 kludge-57 sprocket-60)
          (assemble-order coil-58 unit-55 sprocket-60)
          (assemble-order bracket-59 coil-58 sprocket-60)
          (assemble-order thingumbob-61 contraption-62 frob-63)
          (assemble-order plug-64 widget-18 connector-22)
          (assemble-order device-17 hack-20 connector-22)
          (assemble-order gimcrack-19 hack-20 connector-22)
          (assemble-order gimcrack wire-4 mount-29)
          (assemble-order gimcrack bracket-2 mount-29)
          (assemble-order whatsis-25 coil-27 mount-29)
          (assemble-order doodad-5 widget-14 mount-29)
          (assemble-order hoozawhatsie thingumbob mount-29)
          (assemble-order contraption-13 doodad-5 hoozawhatsie-7)
          (assemble-order contraption-13 device-16 hoozawhatsie-7)
          (assemble-order fastener sprocket hoozawhatsie-7)
          (assemble-order doodad-5 widget-14 hoozawhatsie-7)
          (remove-order widget-14 doodad-5 hoozawhatsie-7)
          (assemble-order sprocket-6 doodad-5 hoozawhatsie-7)
          (assemble-order valve-3 wire-4 contraption)
          (assemble-order valve-3 bracket-2 contraption)
          (assemble-order connector-1 frob-8 contraption)
          (assemble-order gimcrack wire-4 contraption)
          (assemble-order gimcrack bracket-2 contraption)
          (remove-order wire-4 gimcrack contraption)
          (assemble-order fastener sprocket unit)
          (remove-order sprocket fastener unit)
          (assemble-order connector foobar sprocket)
          (assemble-order connector foobar sprocket)
          (assemble-order tube connector sprocket)
          (assemble-order connector foobar fastener)
          (assemble-order connector foobar fastener)
          (remove-order foobar connector fastener)
          (assemble-order hoozawhatsie thingumbob frob)
          (remove-order thingumbob hoozawhatsie frob)
          (assemble-order widget device thingumbob)
          (remove-order device widget thingumbob)
          (assemble-order widget device hoozawhatsie)
          (assemble-order socket hack plug)
          (assemble-order valve socket plug))
   (:goal (complete wire)))