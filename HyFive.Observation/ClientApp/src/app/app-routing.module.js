"use strict";
var __esDecorate = (this && this.__esDecorate) || function (ctor, descriptorIn, decorators, contextIn, initializers, extraInitializers) {
    function accept(f) { if (f !== void 0 && typeof f !== "function") throw new TypeError("Function expected"); return f; }
    var kind = contextIn.kind, key = kind === "getter" ? "get" : kind === "setter" ? "set" : "value";
    var target = !descriptorIn && ctor ? contextIn["static"] ? ctor : ctor.prototype : null;
    var descriptor = descriptorIn || (target ? Object.getOwnPropertyDescriptor(target, contextIn.name) : {});
    var _, done = false;
    for (var i = decorators.length - 1; i >= 0; i--) {
        var context = {};
        for (var p in contextIn) context[p] = p === "access" ? {} : contextIn[p];
        for (var p in contextIn.access) context.access[p] = contextIn.access[p];
        context.addInitializer = function (f) { if (done) throw new TypeError("Cannot add initializers after decoration has completed"); extraInitializers.push(accept(f || null)); };
        var result = (0, decorators[i])(kind === "accessor" ? { get: descriptor.get, set: descriptor.set } : descriptor[key], context);
        if (kind === "accessor") {
            if (result === void 0) continue;
            if (result === null || typeof result !== "object") throw new TypeError("Object expected");
            if (_ = accept(result.get)) descriptor.get = _;
            if (_ = accept(result.set)) descriptor.set = _;
            if (_ = accept(result.init)) initializers.unshift(_);
        }
        else if (_ = accept(result)) {
            if (kind === "field") initializers.unshift(_);
            else descriptor[key] = _;
        }
    }
    if (target) Object.defineProperty(target, contextIn.name, descriptor);
    done = true;
};
var __runInitializers = (this && this.__runInitializers) || function (thisArg, initializers, value) {
    var useValue = arguments.length > 2;
    for (var i = 0; i < initializers.length; i++) {
        value = useValue ? initializers[i].call(thisArg, value) : initializers[i].call(thisArg);
    }
    return useValue ? value : void 0;
};
var __setFunctionName = (this && this.__setFunctionName) || function (f, name, prefix) {
    if (typeof name === "symbol") name = name.description ? "[".concat(name.description, "]") : "";
    return Object.defineProperty(f, "name", { configurable: true, value: prefix ? "".concat(prefix, " ", name) : name });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.AppRoutingModule = void 0;
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var urls_1 = require("./constants/urls");
var loginpage_component_1 = require("./login-page/loginpage.component");
var home_page_observation_component_1 = require("./startside/home-page-observation.component");
var register_four_indications_component_1 = require("./registrering/register-four-indications/register-four-indications.component");
var register_hand_jewelry_component_1 = require("./registrering/register-hand-jewelry/register-hand-jewelry.component");
var register_protective_equipment_component_1 = require("./registrering/register-protective-equipment/register-protective-equipment.component");
var register_glove_component_1 = require("./registrering/register-glove/register-glove.component");
var not_sent_sessions_component_1 = require("./sesjoner/not-sent-sessions/not-sent-sessions.component");
var sent_sessions_component_1 = require("./sesjoner/sent-sessions/sent-sessions.component");
var four_indications_component_1 = require("./sesjoner/fire-indikasjoner/four-indications.component");
var handJewelry_component_1 = require("./sesjoner/handJewelry/handJewelry.component");
var protection_equipment_component_1 = require("./sesjoner/protection-equipment/protection-equipment.component");
var sent_four_indications_session_component_1 = require("./sesjoner/sent-sessions/sent-four-indications-session/sent-four-indications-session.component");
var sent_hand_jewelry_session_component_1 = require("./sesjoner/sent-sessions/sent-hand-jewelry-session/sent-hand-jewelry-session.component");
var sent_protective_equipment_session_component_1 = require("./sesjoner/sent-sessions/sent-protective-equipment-session/sent-protective-equipment-session.component");
var glove_component_1 = require("./sesjoner/glove/glove.component");
var sent_glove_session_component_1 = require("./sesjoner/sent-sessions/sent-glove-session/sent-glove-session.component");
var defaultPath = urls_1.Urls.ProfileUrl;
var routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: defaultPath
    },
    { path: urls_1.Urls.ProfileUrl, component: loginpage_component_1.LoginPageComponent },
    {
        path: urls_1.Urls.HomePageForObservationUrl,
        component: home_page_observation_component_1.HomePageForObservationComponent,
        runGuardsAndResolvers: 'always',
    },
    { path: urls_1.Urls.RegisterFourndicationsUrl, component: register_four_indications_component_1.RegisterFourIndicationsComponent },
    { path: urls_1.Urls.NotSentSessionsUrl, component: not_sent_sessions_component_1.NotSentSessionsComponent },
    { path: urls_1.Urls.SentSessionsUrl, component: sent_sessions_component_1.SentSessionsComponent },
    { path: urls_1.Urls.FourIndicationsSessionUrl, component: four_indications_component_1.FourIndicationsComponent },
    { path: urls_1.Urls.RegisterHandJewelryUrl, component: register_hand_jewelry_component_1.RegisterHandjewelryComponent },
    { path: urls_1.Urls.HandJewelrySessionUrl, component: handJewelry_component_1.HandJewelryComponent },
    { path: urls_1.Urls.RegisterProtectiveEquipmentUrl, component: register_protective_equipment_component_1.RegisterProtectiveEquipmentComponent },
    { path: urls_1.Urls.ProtectiveEquipmentSessionUrl, component: protection_equipment_component_1.ProtectiveEquipmentComponent },
    { path: urls_1.Urls.SentFourIndicationsSessionUrl, component: sent_four_indications_session_component_1.SentFourIndicationsSessionComponent },
    { path: urls_1.Urls.SentHandJewelrySessionUrl, component: sent_hand_jewelry_session_component_1.SentHandJewelrySessionComponent },
    { path: urls_1.Urls.SendProtectiveEquipmentSessionUrl, component: sent_protective_equipment_session_component_1.SentProtectiveEquipmentSessionComponent },
    { path: urls_1.Urls.RegisterGloveUrl, component: register_glove_component_1.RegisterGloveComponent },
    { path: urls_1.Urls.GloveSessionUrl, component: glove_component_1.GloveComponent },
    { path: urls_1.Urls.SentGloveSessionUrl, component: sent_glove_session_component_1.SentGloveSessionComponent },
    {
        path: '**',
        redirectTo: defaultPath
    }
];
var AppRoutingModule = function () {
    var _classDecorators = [(0, core_1.NgModule)({
            imports: [router_1.RouterModule.forRoot(routes, { onSameUrlNavigation: 'reload' })],
            exports: [router_1.RouterModule]
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var AppRoutingModule = _classThis = /** @class */ (function () {
        function AppRoutingModule_1() {
        }
        return AppRoutingModule_1;
    }());
    __setFunctionName(_classThis, "AppRoutingModule");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        AppRoutingModule = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return AppRoutingModule = _classThis;
}();
exports.AppRoutingModule = AppRoutingModule;
//# sourceMappingURL=app-routing.module.js.map