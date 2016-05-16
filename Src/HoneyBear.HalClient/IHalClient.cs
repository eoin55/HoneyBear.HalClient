using System.Collections.Generic;
using HoneyBear.HalClient.Models;

namespace HoneyBear.HalClient
{
    /// <summary>
    /// A lightweight fluent .NET client for navigating and consuming HAL APIs.
    /// </summary>
    public interface IHalClient
    {
        /// <summary>
        /// Returns the most recently navigated resource of the specified type. 
        /// </summary>
        /// <typeparam name="T">The type of the resource to return.</typeparam>
        /// <returns>The most recent navigated resource of the specified type.</returns>
        /// <exception cref="NoActiveResource" />
        IResource<T> Item<T>() where T : class, new();

        /// <summary>
        /// Returns the list of embedded resources in the most recently navigated resource.
        /// </summary>
        /// <typeparam name="T">The type of the resource to return.</typeparam>
        /// <returns>The list of embedded resources in the most recently navigated resource.</returns>
        /// <exception cref="NoActiveResource" />
        IEnumerable<IResource<T>> Items<T>() where T : class, new();

        /// <summary>
        /// Makes a HTTP GET request to the default URI and stores the returned resource.
        /// </summary>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        IHalClient Root();

        /// <summary>
        /// Makes a HTTP GET request to the given URI and stores the returned resource.
        /// </summary>
        /// <param name="href">The URI to request.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        IHalClient Root(string href);

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        IHalClient Get(string rel);

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        IHalClient Get(string rel, string curie);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        IHalClient Get(string rel, object parameters);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        IHalClient Get(string rel, object parameters, string curie);

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        IHalClient Get(IResource resource, string rel);

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        IHalClient Get(IResource resource, string rel, string curie);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        IHalClient Get(IResource resource, string rel, object parameters);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        IHalClient Get(IResource resource, string rel, object parameters, string curie);

        /// <summary>
        /// Makes a HTTP POST request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        IHalClient Post(string rel, object value);

        /// <summary>
        /// Makes a HTTP POST request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        IHalClient Post(string rel, object value, string curie);

        /// <summary>
        /// Makes a HTTP POST request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        IHalClient Post(string rel, object value, object parameters);

        /// <summary>
        /// Makes a HTTP POST request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        IHalClient Post(string rel, object value, object parameters, string curie);

        /// <summary>
        /// Makes a HTTP PUT request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        IHalClient Put(string rel, object value);

        /// <summary>
        /// Makes a HTTP PUT request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        IHalClient Put(string rel, object value, string curie);

        /// <summary>
        /// Makes a HTTP PUT request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        IHalClient Put(string rel, object value, object parameters);

        /// <summary>
        /// Makes a HTTP PUT request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        IHalClient Put(string rel, object value, object parameters, string curie);

        /// <summary>
        /// Makes a HTTP DELETE request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        IHalClient Delete(string rel);

        /// <summary>
        /// Makes a HTTP DELETE request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        IHalClient Delete(string rel, string curie);

        /// <summary>
        /// Makes a HTTP DELETE request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        IHalClient Delete(string rel, object parameters);

        /// <summary>
        /// Makes a HTTP DELETE request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        IHalClient Delete(string rel, object parameters, string curie);

        /// <summary>
        /// Determines whether the most recently navigated resource contains the given link relation.
        /// </summary>
        /// <param name="rel">The link relation to look for.</param>
        /// <returns>Whether or not the link relation exists.</returns>
        bool Has(string rel);

        /// <summary>
        /// Determines whether the most recently navigated resource contains the given link relation.
        /// </summary>
        /// <param name="rel">The link relation to look for.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>Whether or not the link relation exists.</returns>
        bool Has(string rel, string curie);
    }
}