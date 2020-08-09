---
layout: page
title: Donate
---

Do you enjoy Spytify? If so, use [the donation form below](#donate) to offer me:
- A coffee ☕ to keep me awake during those long nights of coding.
- A beer 🍺 to celebrate a new Spytify version that came out.
- More beers 🍻, to let me know that you love Spytify and you want me to keep working hard on it.

Top donations will be added to the donors list using the full name you provided, it doesn't need to be your real name. Don't forget to leave me a message/comment, I love reading those!

<i style="font-size:80%">Transactions are handled by <a href="https://stripe.com/en-ca">Stripe online payments services</a> by default, otherwise you can choose to proceed with <a href="https://www.paypal.com/ca/webapps/mpp/about">Paypal</a> instead of completing the third step.</i>

<!-- ## IssueHunt
Spytify is also on IssueHunt, an issue tracker that lets you **post a bounty on an issue**, a way to attract bounty hunters (devs) attention to get your feature coded or a bug fixed.

Contribute by adding **a bounty 💰** on an issue listed [here](https://issuehunt.io/r/jwallet/spy-spotify?tab=idle) that you wish to see solved or **hunt one 🦉** to be rewarded.

<a href="https://issuehunt.io/r/jwallet/spy-spotify"><img src="./assets/images/isohunt_badge.svg" /></a> -->

[![Donate](https://img.shields.io/badge/Donate via-PayPal.Me-success.svg)](https://www.paypal.com/paypalme/spyspotify)

<!-- ## Donate -->

<article class="donate">
    <section style="display:flex;flex-direction:column;">
        <script src="https://donorbox.org/widget.js" paypalExpress="false"></script><iframe allowpaymentrequest="" frameborder="0" height="900px" name="donorbox" scrolling="no" seamless="seamless" src="https://donorbox.org/embed/spytify" style="width: auto; max-height:none!important"></iframe>
    </section>
    <section>
        <h3>Generous Donors</h3>
        {% assign sortedDonors = site.data.donors | sort: 'donation' | reverse %}
        <ol>
            {% for donor in sortedDonors %}
                {% if forloop.index <= 1 %}
                    <li style="color:#1E963C;"><strong>{{ donor.name }}</strong> - <em>${{ donor.donation}}</em></li>
                {% elsif forloop.index <= 3 %}
                    <li><strong>{{ donor.name }}</strong> - <em>${{ donor.donation}}</em></li>
                {% elsif forloop.index <= 10 %}
                    <li>{{ donor.name }} - <em>${{ donor.donation}}</em></li>
                {% endif %}
            {% endfor %}
        </ol>
        {% for donor in sortedDonors %}
            {% if forloop.index > 100 %}
                {% if forloop.last %}<span style="font-size:90%;color:#ccc;">...</span>{% endif %}
            {% elsif forloop.index > 80 %}
                <span style="font-size:90%;color:#ccc;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% elsif forloop.index > 50 %}
                <span style="font-size:90%;color:#aaa;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% elsif forloop.index > 20 %}
                <span style="font-size:90%;color:#888;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% elsif forloop.index > 10 %}
                <span style="font-size:90%;color:#666;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% endif %}
        {% else %}
            <pre>No Donors</pre>
        {% endfor %}
    </section>
</article>

