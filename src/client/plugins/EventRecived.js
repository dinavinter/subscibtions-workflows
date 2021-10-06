import {EventTypes} from "../../_content/Elsa.Designer.Components.Web/elsa-workflows-studio/index.esm.js";
export function parseJson(json ) {
  if (!json)
    return null;

  try {
    return JSON.parse(json);
  } catch (e) {
    console.warn(`Error parsing JSON: ${e}`);
  }
  return undefined;
}

function PropertyEditorDecoratorPlugin(elsaStudio) {
  const {propertyDisplayManager, getOrCreateProperty} = elsaStudio;
  function defaultSyntax (pd, syntax){return pd.defaultSyntax === syntax;}
  function liquid (pd){return defaultSyntax(pd, "Liquid")}
  function javaScript (pd){return defaultSyntax(pd, "JavaScript")}
  function json (pd){return defaultSyntax(pd, "Json")}
  function variable (pd){return defaultSyntax(pd, "Variable")}

  propertyDisplayManager.originalDisplay = propertyDisplayManager.display;
  propertyDisplayManager.display= displayDecorator;
  function displayDecorator(activity, propertyDescriptor) {
    if ( liquid(propertyDescriptor) || javaScript(propertyDescriptor) || variable(propertyDescriptor) ){
      const propertyModel = getOrCreateProperty(activity, propertyDescriptor.name,()=> propertyDescriptor.defaultValue,()=> propertyDescriptor.defaultSyntax );
      propertyModel.syntax = propertyModel.syntax || propertyDescriptor.defaultSyntax ;
      console.log(propertyModel);
    }
    return  propertyDisplayManager.originalDisplay(activity, propertyDescriptor);
  }
}


export function EventReceivedPlugin(elsaStudio) {
  const {eventBus} = elsaStudio;
  eventBus.on(EventTypes.ActivityDesignDisplaying, onActivityDisplaying);

  function onActivityDisplaying(context) {

    const activityModel = context.activityModel;

    if(activityModel){
      const outcomesSyntax ='Json';
      const outcomes = activityModel.properties.find(x => x.uiHint === 'Outcomes')  ;
      if(outcomes){
        const outcomesExpression = outcomes.expressions[outcomesSyntax] || [];
        context.outcomes = !!outcomesExpression['$values'] ? outcomesExpression['$values'] : Array.isArray(outcomesExpression) ? outcomesExpression : parseJson(outcomesExpression) || [];
      }

      const props = activityModel.properties || [];
      const stateNameProp = props.find(x => x.options === 'DisplayName')
      if(stateNameProp){
        context.displayName = stateNameProp.expressions[stateNameProp.syntax || 'Literal'] || 'EventReceived';
      }


    }

    if (!activityModel || activityModel.type !== 'EventReceived')
      return;

    const props = activityModel.properties || [];
    const stateNameProp = props.find(x => x.name === 'EventName') || {
      name: 'Text',
      expressions: {'Literal': ''},
      syntax: 'Literal'
    };
    context.displayName = stateNameProp.expressions[stateNameProp.syntax || 'Literal'] || 'EventReceived';

    const transitionsSyntax ='Json';
    const transitions = props.find(x => x.name === 'OutcomeNames') || {expressions: {'Json': '[]'}, syntax: transitionsSyntax};
    const transitionsExpression = transitions.expressions[transitionsSyntax] || [];
    context.outcomes = !!transitionsExpression['$values'] ? transitionsExpression['$values'] : Array.isArray(transitionsExpression) ? transitionsExpression : parseJson(transitionsExpression) || [];

  }
}
