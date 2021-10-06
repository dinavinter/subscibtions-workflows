import {  MultiTextDriver} from "../../_content/Elsa.Designer.Components.Web/elsa-workflows-studio/index.esm.js"


export function  DriversPlugin (elsaStudio) {
  const {propertyDisplayManager, getOrCreateProperty} =  elsaStudio;
  propertyDisplayManager.addDriver('outcomes', (elsaStudio) => elsaStudio.propertyDisplayManager.getDriver('multi-text'));

}




// export class MultiTextDriver implements PropertyDisplayDriver {
//     private readonly elsaStudio: ElsaStudio;
//
//     constructor(elsaStudio: ElsaStudio) {
//        this.elsaStudio =elsaStudio;
//      }
//
//   display(activity: ActivityModel, property: ActivityPropertyDescriptor) {
//       const {propertyDisplayManager, getOrCreateProperty} =  this.elsaStudio;
//       const prop =  getOrCreateProperty(activity, property.name);
//     return <elsa-multi-text-property propertyDescriptor={property} propertyModel={prop}/>;
//   }
//
//   update(activity: ActivityModel, property: ActivityPropertyDescriptor, form: FormData) {
//   }
// }
